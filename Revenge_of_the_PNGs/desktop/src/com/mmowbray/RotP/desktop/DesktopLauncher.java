package com.mmowbray.RotP.desktop;

import com.badlogic.gdx.backends.lwjgl.LwjglApplication;
import com.badlogic.gdx.backends.lwjgl.LwjglApplicationConfiguration;
import com.mmowbray.RotP.Game;

public class DesktopLauncher {
	public static void main (String[] arg) {
		LwjglApplicationConfiguration config = new LwjglApplicationConfiguration();
		config.title = "Revenge of the PNGs";
		config.width = 800;
		config.height = 480;
		config.resizable = false;
		new LwjglApplication(new Game(), config);
	}
}
