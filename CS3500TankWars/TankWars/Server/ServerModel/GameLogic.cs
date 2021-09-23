using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TankWars
{
    public class GameLogic
    {

        private World gameWorld;
        private GameSettings gameSettings;
        private GameControlState gameControlState;
        private Random random;
        // used to keep track of when tanks are allowed to shoot again
        private Dictionary<int, int> framesSinceLastFire;
        // used to keep track of when players are allowed to respawn
        private Dictionary<int, int> framesSinceLastDeath;
        // used to keep track of how many powerups a tank has right now
        private Dictionary<int, int> tankCollectedPowerupsCount;
        private int framesSinceLastPowerupSpawned;
        private int framesSinceLastPowerupSpawnedToWait;
        // projectiles technically don't have a size, they're just points.
        // however, we instantiate it as a 1x1 box. this makes detecting collisions easy. 
        // as of right now, the client and server specifications don't allow different projectile sizes, 
        // but in the future it would be cool to have an additional setting to make projectiles bigger
        private const int projectileSize = 3;
        // same thing for powerups. we're just deciding that powerups are a 1x1 box.
        private const int powerupSize = 1;


        public GameLogic(World gameWorld, GameSettings gameSettings, GameControlState gameControlState)
        {
            this.gameWorld = gameWorld;
            this.gameSettings = gameSettings;
            this.gameControlState = gameControlState;
            this.random = new Random();
            this.framesSinceLastFire = new Dictionary<int, int>();
            this.framesSinceLastDeath = new Dictionary<int, int>();
            this.tankCollectedPowerupsCount = new Dictionary<int, int>();
            this.framesSinceLastPowerupSpawned = 0;
            this.framesSinceLastPowerupSpawnedToWait = random.Next(1, gameSettings.MaxPowerupDelay);
        }

        public void CleanupWorld()
        {
            lock (gameWorld) {
                RemoveAllDeadProjectiles();
                CleanupOutOfBoundsProjectiles();
                // we have to send Died=true for exactly one frame to the client and then switch it back to Died=false
                // immediately afterwards, even though technically it's still waiting to respawn.
                SwitchDeadTanksBackToAlive();
                CleanupDisconnectedPlayers();
                CleanupBeams();
                CleanupCollectedPowerups();
            }
        }

        public void UpdateMotion()
        {
            lock (gameWorld) {
                foreach (Tank tank in gameWorld.Tanks.Values) {
                    if (!TankIsZeroHealth(tank)) {
                        Vector2D originalLocation = tank.Location;
                        UpdateTankBody(tank);
                        if (TankIsOutOfWorldBounds(tank, gameWorld)) {
                            TeleportTankToOppositeEdge(tank, gameWorld.Size);
                        }
                        if (TankCollidesWithAnyWalls(tank)) {
                            tank.Location = originalLocation;
                        }
                        UpdateTankTurret(tank);
                    }
                }
                foreach (Projectile projectile in gameWorld.Projectiles.Values) {
                    UpdateProjectilePosition(projectile);
                }
            }
        }

        public void UpdateCollisions()
        {
            lock (gameWorld) {
                HandleProjectileWallCollisions();
                HandleProjectileTankCollisions();
                HandleBeamTankCollisions();
                HandleTankPowerupCollisions();
            }
        }

        public void UpdateGameLogic()
        {
            lock (gameWorld) {
                foreach (Tank tank in gameWorld.Tanks.Values) {
                    if (tank.Joined) {
                        UpdateJustJoinedTank(tank);
                    }
                    UpdateTankFramesSinceLastFireAndDeath(tank);
                    if (TankIsZeroHealth(tank) && TankIsReadyToRespawn(tank)) {
                        RespawnTank(tank);
                    }
                    if (!TankIsZeroHealth(tank)) {
                        UpdateTankFiring(tank);
                    }
                }
                if (gameWorld.Powerups.Count < gameSettings.MaxPowerups) {
                    AttemptToSpawnNewPowerup();
                }
            }
        }

        private void AttemptToSpawnNewPowerup()
        {
            if (framesSinceLastPowerupSpawned > framesSinceLastPowerupSpawnedToWait) {
                AddPowerupToWorld(SpawnNewPowerup());
                framesSinceLastPowerupSpawned = 0;
                framesSinceLastPowerupSpawnedToWait = random.Next(1, gameSettings.MaxPowerupDelay);
            } else {
                framesSinceLastPowerupSpawned++;
            }

        }
        private Powerup SpawnNewPowerup()
        {
            Vector2D location = GetNewRandomLocation();
            Powerup newPowerup = new Powerup(random.Next(), location, false);
            bool powerupSpawnedSafely = false;
            while (!powerupSpawnedSafely) {
                newPowerup.Location = GetNewRandomLocation();
                powerupSpawnedSafely = CheckIfPowerupIsInAllowedLocation(newPowerup);
            }
            return newPowerup;
        }

        private bool CheckIfPowerupIsInAllowedLocation(Powerup powerup)
        {
            bool powerupSpawnedSafely = true;
            foreach (Wall wall in gameWorld.Walls.Values) {
                if (IsPowerupWallCollision(powerup, wall)) {
                    powerupSpawnedSafely = false;
                    break;
                }
            }
            foreach (Tank tank in gameWorld.Tanks.Values) {
                if (IsTankPowerupCollision(tank, powerup)) {
                    powerupSpawnedSafely = false;
                    break;
                }
            }
            powerupSpawnedSafely = powerupSpawnedSafely && !PowerupIsOutOfWorldBounds(powerup, gameWorld);
            return powerupSpawnedSafely;
        }

        private void CleanupOutOfBoundsProjectiles()
        {
            foreach (Projectile projectile in gameWorld.Projectiles.Values) {
                if (ProjectileIsOutOfWorldBounds(projectile, gameWorld)) {
                    gameWorld.Projectiles[projectile.ID].IsDead = true;
                }
            }
        }

        private void SwitchDeadTanksBackToAlive()
        {
            // we have to send Died=true for exactly one frame to the client and then switch it back to Died=false
            // immediately afterwards, even though technically it's still waiting to respawn.
            foreach (Tank tank in gameWorld.Tanks.Values) {
                if (tank.Died) {
                    tank.Died = false;
                }
            }
        }

        private bool ProjectileIsOutOfWorldBounds(Projectile projectile, World gameWorld)
        {
            BoundingBox projectileBounds = new BoundingBox(projectile, projectileSize);
            BoundingBox worldBounds = new BoundingBox(0, 0, gameWorld.Size, gameWorld.Size);
            // if the projectile is "colliding" with the world, then it's inside it, which is okay, so we return the
            // opposite of the collision status
            return !projectileBounds.CollidesWith(worldBounds);
        }

        private bool PowerupIsOutOfWorldBounds(Powerup powerup, World gameWorld)
        {
            BoundingBox powerupBounds = new BoundingBox(powerup, powerupSize);
            BoundingBox worldBounds = new BoundingBox(0, 0, gameWorld.Size, gameWorld.Size);
            // if the powerup is "colliding" with the world, then it's inside it, which is okay, so we return the
            // opposite of the collision status
            return !powerupBounds.CollidesWith(worldBounds);
        }

        private void UpdateTankFramesSinceLastFireAndDeath(Tank tank)
        {
            if (framesSinceLastDeath.ContainsKey(tank.ID)) {
                framesSinceLastDeath[tank.ID]++;
            } else {
                framesSinceLastDeath[tank.ID] = 0;
            }
            if (framesSinceLastFire.ContainsKey(tank.ID)) {
                framesSinceLastFire[tank.ID]++;
            } else {
                framesSinceLastFire[tank.ID] = 0;
            }
        }

        private void UpdateJustJoinedTank(Tank tank)
        {
            if (framesSinceLastDeath[tank.ID] == 0) {
                // we need to send tank.Joined is true to the client once, so leave it as true for now
                RespawnTank(tank);
                tank.Joined = false;
            } else {
                tank.Joined = false;
            }
        }

        private void SetTankJustDied(Tank tank)
        {
            tank.Died = true;
            framesSinceLastDeath[tank.ID] = 0;
        }

        private bool TankIsZeroHealth(Tank tank)
        {
            return tank.HealthPoints <= 0;
        }

        private bool TankIsReadyToRespawn(Tank tank)
        {
            return framesSinceLastDeath[tank.ID] >= gameSettings.RespawnDelay;
        }

        private void RespawnTank(Tank tank)
        {
            // this is a little bit scary because it could potentially be an infinite loop. 
            // however, if this respawn loop is stuck, it's because the game world map is improperly designed,
            // because the walls take up too much space so it's literally impossible for a tank to fit anywhere.
            // so that isn't a bug necessarily, instead it's a limitation when designing maps. 
            bool tankRespawnedSafely = false;
            while (!tankRespawnedSafely) {
                tank.Location = GetNewRandomLocation();
                tankRespawnedSafely = !(TankCollidesWithAnyWalls(tank) || TankIsOutOfWorldBounds(tank, gameWorld));
            }
            tank.HealthPoints = gameSettings.StartingHealthPoints;
        }

        private Vector2D GetNewRandomLocation()
        {
            double xLocation = GetRandomDoubleThatFitsWithinWorldSizeCoordinates(gameWorld.Size);
            double yLocation = GetRandomDoubleThatFitsWithinWorldSizeCoordinates(gameWorld.Size);
            return new Vector2D(xLocation, yLocation);
        }

        private double GetRandomDoubleThatFitsWithinWorldSizeCoordinates(int worldSize)
        {
            double randomDouble = random.NextDouble();
            double scaledDouble = randomDouble * (worldSize);
            double offset = worldSize / 2;
            return scaledDouble - offset;
        }

        private void UpdateTankFiring(Tank tank)
        {
            if (gameControlState.TryGetTankControlCommandByTankID(tank.ID, out TankControlCommand controlCommand)) {
                if (controlCommand.Fire == "main") {
                    if (framesSinceLastFire[tank.ID] > gameSettings.ProjectileFiringDelay) {
                        AddProjectileToWorld(ShootProjectileFromTank(tank));
                        framesSinceLastFire[tank.ID] = 0;
                        lock (gameWorld) {
                            tank.ShotsFired++;
                        }
                    }
                } else if (controlCommand.Fire == "alt") {
                    if (tankCollectedPowerupsCount[tank.ID] > 0) {
                        AddBeamToWorld(ShootBeamFromTank(tank));
                        tankCollectedPowerupsCount[tank.ID]--;
                        lock (gameWorld) {
                            tank.ShotsFired++;
                        }
                    }
                }
            }
            framesSinceLastFire[tank.ID]++;
        }

        private Beam ShootBeamFromTank(Tank tank)
        {
            Beam newBeam = new Beam(random.Next(), tank.Location, tank.TurretDirection, tank.ID);
            return newBeam;
        }

        private Projectile ShootProjectileFromTank(Tank tank)
        {
            Projectile newProjectile = new Projectile(random.Next(), tank.Location, tank.TurretDirection, false, tank.ID);
            return newProjectile;
        }


        private void RemoveAllDeadProjectiles()
        {
            foreach (Projectile projectile in gameWorld.Projectiles.Values.ToList()) {
                if (projectile.IsDead) {
                    gameWorld.Projectiles.Remove(projectile.ID);
                }
            }
        }


        private void HandleProjectileTankCollisions()
        {
            foreach (Projectile projectile in gameWorld.Projectiles.Values) {
                foreach (Tank tank in gameWorld.Tanks.Values) {
                    if (!TankIsZeroHealth(tank)) {
                        if (IsProjectileTankCollision(projectile, tank) && projectile.Owner != tank.ID) {
                            projectile.IsDead = true;
                            gameWorld.Tanks[projectile.Owner].ShotsHit++;
                            tank.HealthPoints--;
                            if (tank.HealthPoints <= 0) {
                                SetTankJustDied(tank);
                                IncreaseScoreForPlayer(projectile.Owner);
                            }
                        }
                    }
                }
            }
        }

        private void HandleTankPowerupCollisions()
        {
            foreach (Tank tank in gameWorld.Tanks.Values) {
                foreach (Powerup powerup in gameWorld.Powerups.Values) {
                    if (IsTankPowerupCollision(tank, powerup)) {
                        powerup.IsDead = true;
                        tankCollectedPowerupsCount[tank.ID]++;
                    }
                }
            }
        }

        private void IncreaseScoreForPlayer(int projectileOwnerID)
        {
            gameWorld.Tanks[projectileOwnerID].Score++;
        }

        private void HandleProjectileWallCollisions()
        {
            foreach (Projectile projectile in gameWorld.Projectiles.Values) {
                foreach (Wall wall in gameWorld.Walls.Values) {
                    if (IsProjectileWallCollision(projectile, wall)) {
                        projectile.IsDead = true;
                    }
                }
            }
        }

        private void HandleBeamTankCollisions()
        {
            foreach (Beam beam in gameWorld.Beams.Values) {
                foreach (Tank tank in gameWorld.Tanks.Values) {
                    if (IsBeamTankCollision(beam, tank) && beam.Owner != tank.ID) {
                        tank.HealthPoints = 0;
                        gameWorld.Tanks[beam.Owner].ShotsHit++;
                        SetTankJustDied(tank);
                        IncreaseScoreForPlayer(beam.Owner);
                    }
                }
            }
        }

        private bool IsProjectileWallCollision(Projectile projectile, Wall wall)
        {
            BoundingBox projectileBounds = new BoundingBox(projectile, projectileSize);
            BoundingBox wallBounds = new BoundingBox(wall, gameSettings.WallSize);
            return projectileBounds.CollidesWith(wallBounds);
        }

        private bool IsProjectileTankCollision(Projectile projectile, Tank tank)
        {
            BoundingBox projectileBounds = new BoundingBox(projectile, projectileSize);
            BoundingBox tankBounds = new BoundingBox(tank, gameSettings.TankSize);
            return projectileBounds.CollidesWith(tankBounds);
        }

        /// <summary>
        /// Determines if a beam intersects a circle where:
        /// the beam is a ray in the context of this method
        /// the tank is a circle in the context of this method 
        /// </summary>
        /// <returns></returns>
        private bool IsBeamTankCollision(Beam beam, Tank tank)
        {
            // ray-circle intersection test
            // P: hit point
            // ray: P = O + tV
            // circle: (P-C)dot(P-C)-r^2 = 0
            // substitute to solve for t gives a quadratic equation:
            // a = VdotV
            // b = 2(O-C)dotV
            // c = (O-C)dot(O-C)-r^2
            // if the discriminant is negative, miss (no solution for P)
            // otherwise, if both roots are positive, hit

            Vector2D rayOrig = beam.Origin;
            Vector2D rayDir = beam.Direction;
            Vector2D center = tank.Location;
            double r = gameSettings.TankSize / 2.0;

            double a = rayDir.Dot(rayDir);
            double b = ((rayOrig - center) * 2.0).Dot(rayDir);
            double c = (rayOrig - center).Dot(rayOrig - center) - r * r;

            // discriminant
            double disc = b * b - 4.0 * a * c;

            if (disc < 0.0)
                return false;

            // find the signs of the roots
            // technically we should also divide by 2a
            // but all we care about is the sign, not the magnitude
            double root1 = -b + Math.Sqrt(disc);
            double root2 = -b - Math.Sqrt(disc);

            return (root1 > 0.0 && root2 > 0.0);
        }



        private void UpdateTankBody(Tank tank)
        {
            if (gameControlState.TryGetTankControlCommandByTankID(tank.ID, out TankControlCommand controlCommand)) {
                UpdateTankBodyPosition(tank, controlCommand);
                UpdateTankBodyDirection(tank, controlCommand);
            }
        }

        private void UpdateTankBodyPosition(Tank tank, TankControlCommand command)
        {
            // update tank location 
            Vector2D movingDirection = new Vector2D(0, 0);
            // tank.BodyDirection = movingDirection;

            movingDirection = ModifyMovingDirection(command.Moving, movingDirection);
            // Build our velocity vector
            Vector2D velocity = movingDirection * gameSettings.TankSpeed;
            // add velocity to position 
            tank.Location = tank.Location + velocity;
        }

        private void UpdateTankBodyDirection(Tank tank, TankControlCommand command)
        {
            Vector2D movingDirection = tank.BodyDirection;
            tank.BodyDirection = ModifyMovingDirection(command.Moving, movingDirection);
        }

        private void UpdateTankTurret(Tank tank)
        {
            if (gameControlState.TryGetTankControlCommandByTankID(tank.ID, out TankControlCommand controlCommand)) {
                tank.TurretDirection = controlCommand.TurretDirection;
            }
        }

        private Vector2D ModifyMovingDirection(string currentMovingDirection, Vector2D movingDirection)
        {
            // If the current moving direction is anything but "none" we will modify the body direction the tank is moving from the default:
            switch (currentMovingDirection) {
                case "up":
                    movingDirection = new Vector2D(0, -1);
                    break;
                case "down":
                    movingDirection = new Vector2D(0, 1);
                    break;
                case "right":
                    movingDirection = new Vector2D(1, 0);
                    break;
                case "left":
                    movingDirection = new Vector2D(-1, 0);
                    break;
            }
            return movingDirection;
        }

        private void UpdateProjectilePosition(Projectile projectile)
        {
            projectile.Location = projectile.Location + (projectile.Direction * gameSettings.ProjectileSpeed);
        }

        private void TeleportTankToOppositeEdge(Tank tank, int worldSize)
        {
            int adjustedSize = worldSize - (2 * gameSettings.TankSize);
            double worldBoundLocation = adjustedSize / 2.0;
            double tankX = tank.Location.GetX();
            double tankY = tank.Location.GetY();
            if (Math.Abs(tankX) > worldBoundLocation) {
                double teleportEdgeOffset = 2 * Math.Sign(tankX);
                tank.Location = new Vector2D(-(tankX - teleportEdgeOffset), tankY);
            }
            if (Math.Abs(tankY) > worldBoundLocation) {
                double teleportEdgeOffset = 2 * Math.Sign(tankY);
                tank.Location = new Vector2D(tankX, -(tankY - teleportEdgeOffset));
            }
        }

        private bool TankIsOutOfWorldBounds(Tank tank, World gameWorld)
        {
            BoundingBox tankBounds = new BoundingBox(tank, gameSettings.TankSize);
            int adjustedWorldSize = gameWorld.Size - (2 * gameSettings.TankSize);
            BoundingBox worldBounds = new BoundingBox(0, 0, adjustedWorldSize, adjustedWorldSize);
            return !tankBounds.CollidesWith(worldBounds);
        }

        private bool TankCollidesWithAnyWalls(Tank tank)
        {
            bool tankCollidesWithAnyWalls = false;
            foreach (Wall wall in gameWorld.Walls.Values) {
                if (IsTankWallCollision(tank, wall)) {
                    tankCollidesWithAnyWalls = true;
                    break;
                }
            }
            return tankCollidesWithAnyWalls;
        }

        private bool IsTankWallCollision(Tank tank, Wall wall)
        {
            BoundingBox tankBounds = new BoundingBox(tank, gameSettings.TankSize);
            BoundingBox wallBounds = new BoundingBox(wall, gameSettings.WallSize);
            return tankBounds.CollidesWith(wallBounds);
        }

        private bool IsTankPowerupCollision(Tank tank, Powerup powerup)
        {
            BoundingBox tankBounds = new BoundingBox(tank, gameSettings.TankSize);
            BoundingBox powerupBounds = new BoundingBox(powerup, powerupSize);
            return tankBounds.CollidesWith(powerupBounds);
        }

        private bool IsPowerupWallCollision(Powerup powerup, Wall wall)
        {
            BoundingBox powerupBounds = new BoundingBox(powerup, powerupSize);
            BoundingBox wallBounds = new BoundingBox(wall, gameSettings.WallSize);
            return powerupBounds.CollidesWith(wallBounds);
        }


        private Tank CreateTankForPlayer(string playerName)
        {
            Tank tank = new Tank();
            // generate a random ID. see this stackoverflow thread for an explanation of why i did it this way: 
            // https://stackoverflow.com/questions/2920696/how-generate-unique-integers-based-on-guids
            tank.ID = random.Next();
            tank.PlayerName = playerName;
            double xLocation = GetRandomDoubleThatFitsWithinWorldSizeCoordinates(gameSettings.UniverseSize);
            double yLocation = GetRandomDoubleThatFitsWithinWorldSizeCoordinates(gameSettings.UniverseSize);
            Vector2D location = new Vector2D(xLocation, yLocation);
            tank.Location = location;
            Vector2D bodyDirection = new Vector2D(1, 0);
            tank.BodyDirection = bodyDirection;
            Vector2D turretDirection = new Vector2D(1, 0);
            tank.TurretDirection = turretDirection;
            tank.Score = 0;
            tank.HealthPoints = gameSettings.StartingHealthPoints;
            tank.Died = false;
            tank.Disconnected = false;
            tank.Joined = true;
            return tank;
        }

        public void InitializeWorld()
        {
            foreach (Wall wall in gameSettings.Walls) {
                AddWallToWorld(wall);
            }
        }

        public int NewPlayerJoined(string playerName)
        {
            Tank newPlayersTank = CreateTankForPlayer(playerName);
            AddTankToWorld(newPlayersTank);
            lock (gameControlState) {
                gameControlState.AddNewPlayer(newPlayersTank.ID);
            }
            return newPlayersTank.ID;
        }

        public void RemovePlayerFromGame(int playerID)
        {
            lock (gameWorld) {
                gameWorld.Tanks[playerID].Disconnected = true;
                gameWorld.Tanks[playerID].Died = true;
                gameWorld.Tanks[playerID].HealthPoints = 0;
            }
        }

        private void CleanupDisconnectedPlayers()
        {
            foreach (Tank tank in gameWorld.Tanks.Values.ToList()) {
                if (tank.Disconnected) {
                    lock (gameWorld) {
                        gameControlState.RemoveDisconnectedPlayer(tank.ID);
                        gameWorld.Tanks.Remove(tank.ID);
                        framesSinceLastDeath.Remove(tank.ID);
                        framesSinceLastFire.Remove(tank.ID);
                        tankCollectedPowerupsCount.Remove(tank.ID);
                    }
                }
            }
        }

        private void CleanupBeams()
        {
            lock (gameWorld) {
                gameWorld.Beams.Clear();
            }
        }

        private void CleanupCollectedPowerups()
        {
            foreach (Powerup powerup in gameWorld.Powerups.Values.ToList()) {
                if (powerup.IsDead) {
                    lock (gameWorld) {
                        gameWorld.Powerups.Remove(powerup.ID);
                    }
                }
            }
        }


        private void AddWallToWorld(Wall wall)
        {
            lock (gameWorld) {
                gameWorld.Walls[wall.ID] = wall;
            }
        }

        private void AddTankToWorld(Tank tank)
        {
            lock (gameWorld) {
                gameWorld.Tanks[tank.ID] = tank;
                framesSinceLastDeath[tank.ID] = 0;
                framesSinceLastFire[tank.ID] = 0;
                tankCollectedPowerupsCount[tank.ID] = 0;
            }
        }

        private void AddProjectileToWorld(Projectile projectile)
        {
            lock (gameWorld) {
                gameWorld.Projectiles[projectile.ID] = projectile;
            }
        }

        private void AddBeamToWorld(Beam beam)
        {
            lock (gameWorld) {
                gameWorld.Beams[beam.ID] = beam;
            }
        }

        private void AddPowerupToWorld(Powerup powerup)
        {
            lock (gameWorld) {
                gameWorld.Powerups[powerup.ID] = powerup;
            }
        }
    }
}
