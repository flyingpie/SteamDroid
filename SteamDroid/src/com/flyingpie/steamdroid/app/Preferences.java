package com.flyingpie.steamdroid.app;

import android.os.Bundle;
import android.preference.PreferenceActivity;

import com.flyingpie.steamdroid.R;

public class Preferences extends PreferenceActivity {

	@Override
	protected void onCreate(Bundle savedInstanceState) {
		super.onCreate(savedInstanceState);
		addPreferencesFromResource(R.layout.preferences);
	}	
}
