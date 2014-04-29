using System;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Revenge_of_the_PNGs
{
	public class Projectile
	{
	    public Texture2D projectileTexture;
	    public Vector2 projectilePosition;
	    public Boolean active;

	    public int dX, dY;

	    public Projectile(Texture2D projectileTexture, Vector2 projectilePosition, int dX, int dY) //Initializes our projectile
	    {
	        this.projectileTexture = projectileTexture;
	        this.projectilePosition = projectilePosition;
	        this.active = true; //new projectile should be active
	        this.dX = dX;
	        this.dY = dY;
	    }

	    public void Update()
	    {
	        if (active)
	        {
	            projectilePosition.X += dX;
	            projectilePosition.Y += dY;
	        }
	    }

	    public void Draw(SpriteBatch spriteBatch)
	    {
	        if (active)
	        {
	            spriteBatch.Begin();
	            spriteBatch.Draw(projectileTexture, projectilePosition, Color.White);
	            spriteBatch.End();
	        }
	    }
	    public void killProjectile()
	    {
	        this.active = false;
	    }
	}
}

