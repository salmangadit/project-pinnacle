package com.example.scope;

import java.io.File;
import java.io.IOException;

import android.media.ExifInterface;
import android.net.Uri;
import android.os.Bundle;
import android.os.Environment;
import android.provider.MediaStore;
import android.app.ActionBar;
import android.app.Activity;
import android.app.Fragment;
import android.content.Context;
import android.content.Intent;
import android.graphics.Bitmap;
import android.graphics.BitmapFactory;
import android.graphics.Matrix;
import android.util.Log;
import android.view.Menu;
import android.view.View;
import android.widget.Button;
import android.widget.Toast;

public class MainActivity extends Activity {
	protected static String _path;
	protected boolean _taken;

	public static final String DATA_PATH = Environment
			.getExternalStorageDirectory().toString() + "/Scope/";
	private static final String TAG = "Scope.java";

	protected static final String PHOTO_TAKEN = "photo_taken";
	public static Context appContext;

	@Override
	public void onCreate(Bundle savedInstanceState) {
		super.onCreate(savedInstanceState);
		
		//Create any missing directories
		String[] paths = new String[] { DATA_PATH, DATA_PATH + "tessdata/" };

		for (String path : paths) {
			File dir = new File(path);
			if (!dir.exists()) {
				if (!dir.mkdirs()) {
					Log.v(TAG, "ERROR: Creation of directory " + path + " on sdcard failed");
					return;
				} else {
					Log.v(TAG, "Created directory " + path + " on sdcard");
				}
			}

		}
		
		
		setContentView(R.layout.activity_main);
		// ActionBar gets initiated
		ActionBar actionbar = getActionBar();

		// Tell the ActionBar we want to use Tabs.
		actionbar.setNavigationMode(ActionBar.NAVIGATION_MODE_TABS);

		// initiating both tabs and set text to it.
		ActionBar.Tab ScanTab = actionbar.newTab().setText("SCAN");
		ActionBar.Tab SettingsTab = actionbar.newTab().setText("SETTINGS");

		// create the two fragments we want to use for display content
		Fragment ScanFragment = new ScanFragment();
		Fragment SettingsFragment = new SettingsSetterFragment();

		// set the Tab listener. Now we can listen for clicks.
		ScanTab.setTabListener(new TabListener(ScanFragment));
		SettingsTab.setTabListener(new TabListener(SettingsFragment));

		// add the two tabs to the actionbar
		actionbar.addTab(ScanTab);
		actionbar.addTab(SettingsTab);
	}

	

	@Override
	public boolean onCreateOptionsMenu(Menu menu) {
		getMenuInflater().inflate(R.menu.activity_main, menu);
		return true;
	}
}
