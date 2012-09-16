package com.example.scope;

import java.io.File;
import java.io.IOException;

import com.example.scope.MainActivity;

import android.app.Fragment;
import android.content.Context;
import android.content.Intent;
import android.graphics.Bitmap;
import android.graphics.BitmapFactory;
import android.graphics.Matrix;
import android.media.ExifInterface;
import android.net.Uri;
import android.os.Bundle;
import android.os.Environment;
import android.provider.MediaStore;
import android.util.Log;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.Button;
import android.widget.Toast;

public class ScanFragment extends Fragment {
	protected Button _button;
	protected static String _path;
	protected boolean _taken;

	public static final String DATA_PATH = Environment
			.getExternalStorageDirectory().toString() + "/Scope/";
	private static final String TAG = "Scope.java";

	protected static final String PHOTO_TAKEN = "photo_taken";

	@Override
	public View onCreateView(LayoutInflater inflater, ViewGroup container,
			Bundle savedInstanceState) {
		// Inflate the layout for this fragment
		View view = inflater.inflate(R.layout.scanfragment, container, false);

		// _image = (ImageView) findViewById(R.id.image);
		// _field = (EditText) findViewById(R.id.field);
		_button = (Button) view.findViewById(R.id.button_scan);

		Log.v(TAG, "HERE1");
		if (_button == null)
			Log.v(TAG, "Null bitch");
		_button.setOnClickListener(new ButtonClickHandler());
		_path = DATA_PATH + "/ocr.jpg";
		Log.v(TAG, "HERE2");
		return view;
	}

	public class ButtonClickHandler implements View.OnClickListener {
		public void onClick(View view) {
			Log.v(TAG, "Starting Camera app");
			startCameraActivity();
		}
	}
	
	public void startCameraActivity() {
		File file = new File(_path);
		Uri outputFileUri = Uri.fromFile(file);

		final Intent intent = new Intent(MediaStore.ACTION_IMAGE_CAPTURE);
		intent.putExtra(MediaStore.EXTRA_OUTPUT, outputFileUri);

		startActivityForResult(intent, 0);
	}

	@Override
	public void onActivityResult(int requestCode, int resultCode, Intent data) {

		Log.i(TAG, "resultCode: " + resultCode);

		if (resultCode == -1) {
			onPhotoTaken();
		} else {
			Log.v(TAG, "User cancelled");
		}
	}

	@Override
	public void onSaveInstanceState(Bundle outState) {
		outState.putBoolean(MainActivity.PHOTO_TAKEN, _taken);
	}

	protected void onRestoreInstanceState(Bundle savedInstanceState) {
		Log.i(TAG, "onRestoreInstanceState()");
		if (savedInstanceState.getBoolean(MainActivity.PHOTO_TAKEN)) {
			onPhotoTaken();
		}
	}

	protected void onPhotoTaken() {
		_taken = true;

		BitmapFactory.Options options = new BitmapFactory.Options();
		options.inSampleSize = 4;

		Bitmap bitmap = BitmapFactory.decodeFile(_path, options);
		if (bitmap == null)
			Log.v(TAG, _path);
		try {
			ExifInterface exif = new ExifInterface(_path);
			int exifOrientation = exif.getAttributeInt(
					ExifInterface.TAG_ORIENTATION,
					ExifInterface.ORIENTATION_NORMAL);

			Log.v(TAG, "Orient: " + exifOrientation);

			int rotate = 0;

			switch (exifOrientation) {
			case ExifInterface.ORIENTATION_ROTATE_90:
				rotate = 90;
				break;
			case ExifInterface.ORIENTATION_ROTATE_180:
				rotate = 180;
				break;
			case ExifInterface.ORIENTATION_ROTATE_270:
				rotate = 270;
				break;
			}

			Log.v(TAG, "Rotation: " + rotate);

			if (rotate != 0) {

				// Getting width & height of the given image.
				int w = bitmap.getWidth();
				int h = bitmap.getHeight();

				// Setting pre rotate
				Matrix mtx = new Matrix();
				mtx.preRotate(rotate);

				// Rotating Bitmap
				bitmap = Bitmap.createBitmap(bitmap, 0, 0, w, h, mtx, false);
			}
			Log.v(TAG, "1");
			// Convert to ARGB_8888, required by tess
			bitmap = bitmap.copy(Bitmap.Config.ARGB_8888, true);
			Log.v(TAG, "2");
		} catch (IOException e) {
			Log.e(TAG, "Couldn't correct orientation: " + e.toString());
		}

		Context context = getActivity().getApplicationContext();
		CharSequence text = "Photo taken! Now OCR that shiz!";
		int duration = Toast.LENGTH_SHORT;

		Toast toast = Toast.makeText(context, text, duration);
		toast.show();
	}
}
