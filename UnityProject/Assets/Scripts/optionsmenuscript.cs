using UnityEngine;
using System.Collections;

public class optionsmenuscript : MonoBehaviour {
	public GUISkin skin;
	public Rect windowRect;
	public int menu_width= 300;
	public int menu_height= 500;
	public bool show_menu= false;

	void LockCursor() {
		Cursor.visible = false;
		Cursor.lockState = CursorLockMode.Locked;
	}

	void UnlockCursor() {
		Cursor.lockState = CursorLockMode.None;
		Cursor.visible = true;
	}

	void  OnApplicationPause (){  
		UnlockCursor ();
	}

	void  OnApplicationFocus (){
		if(!show_menu){
			LockCursor();
		}
	}

	void  ShowMenu (){
		show_menu = true;
	}

	void  HideMenu (){
		show_menu = false;
	}

	void  OnGUI (){
		if(!show_menu){
			return;
		}
		windowRect = GUI.Window (
			0, 
			new Rect(Screen.width*0.5f - menu_width*0.5f, Screen.height*0.5f - menu_height*0.5f, menu_width, menu_height), 
			WindowFunction, 
			"",
			skin.window);
	}

	private Vector2 draw_cursor;
	private Vector2 draw_cursor_line;
	private int line_height= 24;
	private int line_offset= 24;

	void  DrawCursor_Init (){
		draw_cursor = new Vector2(25,25);
		draw_cursor_line = new Vector2(0,0);	
	}

	void  DrawCursor_NextLine (){
		draw_cursor_line = new Vector2(0,0);	
		draw_cursor.y += line_offset;
	}

	void  DrawCursor_Offset ( float val  ){
		draw_cursor_line.x += val;	
	}

	Vector2 DrawCursor_Get (){
		return draw_cursor + draw_cursor_line;
	}

	Rect DrawCursor_RectSpace ( float width  ){
		var rect= new Rect(draw_cursor.x + draw_cursor_line.x,
						draw_cursor.y + draw_cursor_line.y,
						width,
						line_height);
		DrawCursor_Offset(width);
		return rect;
	}

	void  DrawLabel ( string text  ){
		DrawCursor_Offset(17);
		GUI.Label (
			DrawCursor_RectSpace(400),//GUI.skin.label.CalcSize(new GUIContent(text)).x), 
			text, 
			skin.label);
	}

	bool DrawCheckbox (  bool val ,    string text   ){
		 val = GUI.Toggle (
			DrawCursor_RectSpace(400), 
			val,
			text, 
			skin.toggle);
		return val;			
	}

	float DrawSlider (  float val  ){
		DrawCursor_Offset(18);
		val = GUI.HorizontalSlider (
			DrawCursor_RectSpace(400 - draw_cursor_line.x), 
			val,
			0.0f, 
			1.0f);
		return val;			
	}

	bool DrawButton (  string text  ){
		 var val= GUI.Button (
			DrawCursor_RectSpace(200), 
			text,
			skin.button);
		return val;			
	}

	//private floatbrightness= 0.3f;
	private float master_volume= 1.0f;
	private float sound_volume= 1.0f;
	private float music_volume= 1.0f;
	private float voice_volume= 1.0f;
	private bool lock_gun_to_center= false;
	private bool mouse_invert= false;
	private float mouse_sensitivity= 0.2f;
	private bool show_advanced_sound= false;
	private bool toggle_crouch= true;
	private Vector2 scroll_view_vector= new Vector2(0,0);
	private float vert_scroll= 0.0f;
	private float vert_scroll_pixels= 0.0f;
	private float gun_distance= 1.0f;
	 
	void  RestoreDefaults (){
		master_volume = 1.0f;
		sound_volume = 1.0f;
		music_volume = 1.0f;
		voice_volume = 1.0f;
		mouse_sensitivity = 0.2f;
		lock_gun_to_center = false;
		mouse_invert = false;
		toggle_crouch = true;      
	}

	void  Start (){
		LockCursor ();
		RestoreDefaults();
		master_volume = PlayerPrefs.GetFloat("master_volume", master_volume);
		sound_volume = PlayerPrefs.GetFloat("sound_volume", sound_volume);
		music_volume = PlayerPrefs.GetFloat("music_volume", music_volume);
		voice_volume = PlayerPrefs.GetFloat("voice_volume", voice_volume);
		mouse_sensitivity = PlayerPrefs.GetFloat("mouse_sensitivity", mouse_sensitivity);
		lock_gun_to_center = PlayerPrefs.GetInt("lock_gun_to_center", lock_gun_to_center?1:0)==1;
		mouse_invert = PlayerPrefs.GetInt("mouse_invert", mouse_invert?1:0)==1;
		toggle_crouch = PlayerPrefs.GetInt("toggle_crouch", toggle_crouch?1:0)==1;      
		gun_distance = PlayerPrefs.GetFloat("gun_distance", 1.0f);       
	}
	 

	void  SavePrefs (){
		PlayerPrefs.SetFloat("master_volume", master_volume);
		PlayerPrefs.SetFloat("sound_volume", sound_volume);
		PlayerPrefs.SetFloat("music_volume", music_volume);
		PlayerPrefs.SetFloat("voice_volume", voice_volume);
		PlayerPrefs.SetFloat("mouse_sensitivity", mouse_sensitivity);
		PlayerPrefs.SetInt("lock_gun_to_center", lock_gun_to_center?1:0);
		PlayerPrefs.SetInt("mouse_invert", mouse_invert?1:0);
		PlayerPrefs.SetInt("toggle_crouch", toggle_crouch?1:0);    
		PlayerPrefs.SetFloat("gun_distance", gun_distance);    
	}

	public bool IsMenuShown (){
		 return show_menu;
	}

	void  Update (){
		if(vert_scroll != -1.0f){
			vert_scroll += Input.GetAxis("Mouse ScrollWheel");
		}
		if(Input.GetKeyDown ("escape") && !show_menu){
			UnlockCursor();
			ShowMenu();
		} else if(Input.GetKeyDown ("escape") && show_menu){
			LockCursor();
			HideMenu();
		}
		if(Input.GetMouseButtonDown(0) && !show_menu){
			LockCursor();
		}
		if(show_menu){
			Time.timeScale = 0.0f;
		} else {
			if(Time.timeScale == 0.0f){
				Time.timeScale = 1.0f;
			}
		}
	}

	void  WindowFunction ( int windowID  ){

		scroll_view_vector = GUI.BeginScrollView ( new Rect(0, 0, windowRect.width, windowRect.height), 
			scroll_view_vector, 
			new Rect (0, vert_scroll_pixels, windowRect.width, windowRect.height));

		DrawCursor_Init();
		mouse_invert = DrawCheckbox(mouse_invert, "Invert mouse");
		DrawCursor_NextLine();
		DrawLabel("Mouse sensitivity:");
		DrawCursor_NextLine();
		mouse_sensitivity = DrawSlider(mouse_sensitivity);
		DrawCursor_NextLine();
		DrawLabel("Distance from eye to gun:");
		DrawCursor_NextLine();
		gun_distance = DrawSlider(gun_distance);
		DrawCursor_NextLine();
		toggle_crouch = DrawCheckbox(toggle_crouch, "Toggle crouch");
		DrawCursor_NextLine();
		lock_gun_to_center = DrawCheckbox(lock_gun_to_center, "Lock gun to screen center");
		DrawCursor_NextLine();
		/*DrawLabel("Brightness:");
		DrawCursor_NextLine();
		brightness = DrawSlider(brightness);
		DrawCursor_NextLine();*/
		DrawLabel("Master volume:");
		DrawCursor_NextLine();
		master_volume = DrawSlider(master_volume);
		DrawCursor_NextLine();
		show_advanced_sound = DrawCheckbox(show_advanced_sound, "Advanced sound options");
		DrawCursor_NextLine();
		if(show_advanced_sound){
			var indent= 44;
			DrawLabel("....Sounds:");
			DrawCursor_NextLine();
			DrawCursor_Offset(indent);
			sound_volume = DrawSlider(sound_volume);
			DrawCursor_NextLine();
			DrawLabel("....Voice:");
			DrawCursor_NextLine();
			DrawCursor_Offset(indent);
			voice_volume = DrawSlider(voice_volume);
			DrawCursor_NextLine();
			DrawLabel("....Music:");
			DrawCursor_NextLine();
			DrawCursor_Offset(indent);
			music_volume = DrawSlider(music_volume);
			DrawCursor_NextLine();
		}
		if(DrawButton("Resume")){
			LockCursor();
			show_menu = false;
		}
		draw_cursor.y += line_offset * 0.3f;
		DrawCursor_NextLine();
		if(DrawButton("Restore defaults")){
			RestoreDefaults();
		}
		DrawCursor_NextLine();	
		draw_cursor.y += line_offset * 0.3f;
		if(DrawButton("Exit game")){
			Application.Quit();
		}
		
		var content_height= draw_cursor.y;
		
		GUI.EndScrollView();
		
		if(content_height > windowRect.height){
			var leeway= (content_height / windowRect.height - windowRect.height / content_height);
			if(vert_scroll == -1.0f){
				vert_scroll = leeway;
			}
			vert_scroll = GUI.VerticalScrollbar ( new Rect(menu_width-20, 20, menu_width, menu_height-25), 
				vert_scroll, 
				windowRect.height / content_height,
				content_height / windowRect.height, 
				0.0f);
			vert_scroll_pixels = windowRect.height * (leeway - vert_scroll);
		} else {
			vert_scroll = -1.0f;
			vert_scroll_pixels = 0.0f;
		}
		SavePrefs();
	}
}