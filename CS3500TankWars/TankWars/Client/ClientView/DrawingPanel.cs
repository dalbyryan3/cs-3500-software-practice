// Luke Ludlow, Ryan Dalby, CS 3500 Fall 2019
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TankWars
{
    /// <summary>
    /// this is the drawing panel for the game, it's the actual game window view. 
    /// the view is always drawn at 800x800 pixels, regardless of world size. 
    /// 
    /// the "main" method of the game drawing panel is OnPaint. OnPaint is indirectly 
    /// invoked by the game controller, which invalidates the view in order to trigger OnPaint.
    /// this means that the controller is in control (no pun intended) of refreshing the view.
    /// 
    /// all of the particular drawing details are delegated to other classes (DrawingTransformer, TankDrawer, etc.). 
    /// 
    /// additionally, the drawing panel is responsible for remembering and refreshing animations. 
    /// the individual drawers for explosions, powerups, and beams do the actual image animator calls.
    /// drawing panel instantiates a new drawer for explosions, powerups, and beams so that we can do multiple animations
    /// simultaneously in multiple states (different timings and locations).
    /// </summary>
    public class DrawingPanel : Panel
    {

        private World gameWorld;
        private GameController gameController;
        private PlayerColorManager playerColorManager;
        private TankDrawer tankDrawer;
        private BackgroundDrawer backgroundDrawer;
        private WallDrawer wallDrawer;
        private ProjectileDrawer projectileDrawer;

        Dictionary<int, PowerupDrawer> powerupAnimationDrawers;
        List<BeamDrawer> beamAnimationDrawers;
        List<ExplosionDrawer> explosionAnimationDrawers;


        public DrawingPanel(GameController gameController)
        {
            DoubleBuffered = true;

            playerColorManager = new PlayerColorManager();

            this.gameController = gameController;
            this.gameWorld = gameController.theWorld;
            tankDrawer = new TankDrawer(playerColorManager);
            backgroundDrawer = new BackgroundDrawer();
            wallDrawer = new WallDrawer();
            projectileDrawer = new ProjectileDrawer(playerColorManager);

            powerupAnimationDrawers = new Dictionary<int, PowerupDrawer>();
            beamAnimationDrawers = new List<BeamDrawer>();
            explosionAnimationDrawers = new List<ExplosionDrawer>();

        }
        
        protected override void OnPaint(PaintEventArgs e)
        {

            e.Graphics.Clear(Color.Black);

            if (gameWorld.Tanks.Count <= 0) {
                // don't draw anything. just black screen
                return;
            }

            DrawingTransformer.TranslateTransformToCenterPlayersView(this.Size.Width, gameWorld.Size, gameController.GetPlayerLocation(), e);

            DrawWorld(gameWorld, e);

            base.OnPaint(e);
        }

        private void DrawWorld(World world, PaintEventArgs e)
        {
            lock (gameWorld) {
                DrawBackground(e, world.Size);
                DrawWalls(world.Walls, e, world.Size);
                DrawTanks(world.Tanks, e, world.Size);
                DrawProjectiles(world.Projectiles, e, world.Size);
                DrawBeams(world.Beams, e, world.Size);
                DrawPowerups(world.Powerups, e, world.Size);

                RefreshPowerupAnimations(e);
                RefreshBeamAnimations(e);
                RefreshExplosionAnimations(e);
            }
        }

        private void RefreshExplosionAnimations(PaintEventArgs e)
        {
            foreach (ExplosionDrawer explosionAnimation in explosionAnimationDrawers) {
                explosionAnimation.ContinueDrawingExplosion(e, gameWorld.Size);
            }
        }

        private void RefreshPowerupAnimations(PaintEventArgs e)
        {
            RemoveDeadPowerups();
            foreach (PowerupDrawer powerupAnimation in powerupAnimationDrawers.Values) {
                powerupAnimation.ContinueDrawingPowerup(e, gameWorld.Size);
            }
        }

        private void RefreshBeamAnimations(PaintEventArgs e)
        {
            foreach (BeamDrawer beamAnimation in beamAnimationDrawers) {
                beamAnimation.ContinueDrawingBeam(e, gameWorld.Size);
            }
        }

        private void DrawBackground(PaintEventArgs e, int worldSize)
        {
            backgroundDrawer.DrawBackground(e, worldSize);
        }

        private void DrawWalls(Dictionary<int, Wall> walls, PaintEventArgs e, int worldSize)
        {
            foreach (Wall wall in walls.Values) {
                wallDrawer.DrawWall(wall, e, worldSize);
            }
        }

        private void DrawTanks(Dictionary<int, Tank> tanks, PaintEventArgs e, int worldSize)
        {
            foreach (Tank tank in tanks.Values) {
                if (tank.Died) {
                    explosionAnimationDrawers.Add(new ExplosionDrawer(this, tank));
                } else {
                    if (tank.IsZeroHealth()) {
                        // The tank will have an HP of 0 while it's dead. When it has an HP of 3 again, that's how you know when it has respawned. 
                        // In other words, just don't draw tanks that have an HP of 0.
                        // (the server only sends "Died" for one frame, so we can't use that as an indicator of when to not draw the tank.)
                    } else {
                        tankDrawer.DrawTank(tank, e, worldSize);
                    }
                }
            }
        }

        private void DrawProjectiles(Dictionary<int, Projectile> projectiles, PaintEventArgs e, int worldSize)
        {
            foreach (Projectile projectile in projectiles.Values) {
                projectileDrawer.DrawProjectile(projectile, e, worldSize);
            }
        }

        private void DrawBeams(Dictionary<int, Beam> beams, PaintEventArgs e, int worldSize)
        {
            foreach (Beam beam in beams.Values) {
                beamAnimationDrawers.Add(new BeamDrawer(this, beam));
            }
        }

        private void DrawPowerups(Dictionary<int, Powerup> powerups, PaintEventArgs e, int worldSize)
        {
            foreach (Powerup powerup in powerups.Values) {
                RemoveDeadPowerups();
                if (!powerupAnimationDrawers.ContainsKey(powerup.ID)) {
                    powerupAnimationDrawers[powerup.ID] = new PowerupDrawer(this, powerup);
                }
            }
        }

        private void RemoveDeadPowerups()
        {
            foreach (int powerupID in powerupAnimationDrawers.Keys.ToList()) {
                if (!gameWorld.Powerups.ContainsKey(powerupID)) {
                    powerupAnimationDrawers.Remove(powerupID);
                }
            }
        }

    }
}
