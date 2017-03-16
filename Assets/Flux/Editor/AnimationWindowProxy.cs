using UnityEngine;
using UnityEditor;

using System;
using System.Reflection;

namespace FluxEditor
{
	public class AnimationWindowProxy {

		private static Type ANIMATION_WINDOW_TYPE = typeof(EditorWindow).Assembly.GetType("UnityEditor.AnimationWindow");
		private static Type ANIMATION_WINDOW_STATE_TYPE = typeof(EditorWindow).Assembly.GetType("UnityEditorInternal.AnimationWindowState");
		private static Type ANIMATION_SELECTION_TYPE = typeof(EditorWindow).Assembly.GetType("UnityEditor.AnimationSelection");

#if !UNITY_5_0
		private static Type ANIMATION_EDITOR_TYPE = typeof(EditorWindow).Assembly.GetType("UnityEditor.AnimEditor");
#endif

		private static EditorWindow _animationWindow = null;
		public static EditorWindow AnimationWindow {
			get	{
				if( _animationWindow == null )
					_animationWindow = FUtility.GetWindowIfExists( ANIMATION_WINDOW_TYPE );
				return _animationWindow;
			}
		}

		public static EditorWindow OpenAnimationWindow()
		{
			if( _animationWindow == null )
				_animationWindow = EditorWindow.GetWindow( ANIMATION_WINDOW_TYPE );
			return _animationWindow;
		}

		#region AnimationWindow variables

#if !UNITY_5_0
		private static FieldInfo _animEditorField = null;
		private static FieldInfo AnimEditorField {
			get {
				if( _animEditorField == null )
					_animEditorField = ANIMATION_WINDOW_TYPE.GetField( "m_AnimEditor", BindingFlags.Instance | BindingFlags.NonPublic );
				return _animEditorField;
			}
		}

		private static ScriptableObject _animEditor = null;
		private static ScriptableObject AnimEditor {
			get {
				if( _animEditor == null )
					_animEditor = (ScriptableObject)AnimEditorField.GetValue( AnimationWindow );
				return _animEditor;
			}
		}
#endif

#if UNITY_5_0
		private static PropertyInfo _stateProperty = null;
		private static PropertyInfo StateProperty
		{
			get{
				if( _stateProperty == null )
					_stateProperty = ANIMATION_WINDOW_TYPE.GetProperty("state", BindingFlags.Instance | BindingFlags.Public);
				return _stateProperty;
			}
		}

		private static FieldInfo _selectedAnimationField = null;
		private static FieldInfo SelectedAnimationField {
			get {
				if( _selectedAnimationField == null )
					_selectedAnimationField = ANIMATION_WINDOW_TYPE.GetField("m_Selected");
				return _selectedAnimationField;
			}
		}

		private static MethodInfo _beginAnimationMode = null;
		public static MethodInfo BeginAnimationMode
		{
			get {
				if( _beginAnimationMode == null )
					_beginAnimationMode = ANIMATION_WINDOW_TYPE.GetMethod("BeginAnimationMode", BindingFlags.Instance | BindingFlags.Public, null, new Type[]{typeof(bool)}, null );
				return _beginAnimationMode;
			}
		}

		private static MethodInfo _previewFrame = null;
		public static MethodInfo PreviewFrame
		{
			get {
				if( _previewFrame == null )
					_previewFrame = ANIMATION_WINDOW_TYPE.GetMethod("PreviewFrame", BindingFlags.Instance | BindingFlags.Public, null, new Type[]{typeof(int)}, null );
				return _previewFrame;
			}
		}
#else
		private static FieldInfo _stateField = null;
		private static FieldInfo StateField {
			get {
				if( _stateField == null )
					_stateField = ANIMATION_EDITOR_TYPE.GetField("m_State", BindingFlags.Instance | BindingFlags.NonPublic);
				return _stateField;
			}

		}

		private static PropertyInfo _activeAnimationClipProperty = null;
		private static PropertyInfo ActiveAnimationClipProperty {
			get {
				if( _activeAnimationClipProperty == null )
					_activeAnimationClipProperty = ANIMATION_WINDOW_STATE_TYPE.GetProperty("activeAnimationClip", BindingFlags.Instance | BindingFlags.Public);
				return _activeAnimationClipProperty;
			}
		}
#endif

		#endregion

		#region AnimationWindowState variables

#if UNITY_5_0
		private static FieldInfo _timeField = null;
		private static FieldInfo TimeField { 
			get {
				if( _timeField == null )
					_timeField = ANIMATION_WINDOW_STATE_TYPE.GetField("m_PlayTime");
				return _timeField;
			}
		}

		private static FieldInfo _frameField = null;
		private static FieldInfo FrameField {
			get {
				if( _frameField == null )
					_frameField = ANIMATION_WINDOW_STATE_TYPE.GetField("m_Frame");
				return _frameField;
			}
		}
#else
		private static FieldInfo _currentTimeField = null;
		private static FieldInfo CurrentTimeField {
			get {
				if( _currentTimeField == null )
					_currentTimeField = ANIMATION_WINDOW_STATE_TYPE.GetField("m_CurrentTime", BindingFlags.Instance | BindingFlags.NonPublic);
				return _currentTimeField;
			}
		}
		
		private static PropertyInfo _frameProperty = null;
		private static PropertyInfo FrameProperty {
			get {
				if( _frameProperty == null )
					_frameProperty = ANIMATION_WINDOW_STATE_TYPE.GetProperty("frame", BindingFlags.Instance | BindingFlags.Public);
				return _frameProperty;
			}
		}

		private static PropertyInfo _recordingProperty = null;
		private static PropertyInfo RecordingProperty {
			get {
				if( _recordingProperty == null )
					_recordingProperty = ANIMATION_WINDOW_STATE_TYPE.GetProperty("recording", BindingFlags.Instance | BindingFlags.Public);
				return _recordingProperty;
			}
		}
#endif

		#endregion

		#region AnimationSelection variables
		private static MethodInfo _chooseClipMethod = null;
		private static MethodInfo ChooseClipMethod {
			get {
				if( _chooseClipMethod == null )
					_chooseClipMethod = ANIMATION_SELECTION_TYPE.GetMethod( "ChooseClip", BindingFlags.Instance | BindingFlags.NonPublic, null, new Type[]{typeof(AnimationClip)}, null );
				return _chooseClipMethod;
			}
		}
		#endregion

		public static void StartAnimationMode()
		{
//			MethodInfo onSelectionChange = ANIMATION_WINDOW_TYPE.GetMethod( "OnSelectionChange", BindingFlags.Instance | BindingFlags.Public, null, new Type[0], null );
//			onSelectionChange.Invoke(AnimationWindow, null);
//			object[] selectedAnimation = (object[])SelectedAnimationField.GetValue( AnimationWindow );
//			Transform t = Selection.activeTransform;
//			Selection.activeTransform = null;
//			Selection.activeTransform = t;
//			MethodInfo reenterAnimationMode = ANIMATION_WINDOW_TYPE.GetMethod( "ReEnterAnimationMode", BindingFlags.Instance | BindingFlags.Public, null, new Type[0], null );
//			reenterAnimationMode.Invoke( AnimationWindow, null );
#if UNITY_5_0
			BeginAnimationMode.Invoke( AnimationWindow, new object[]{false} );
#else
			RecordingProperty.SetValue( GetState(), true, null );
#endif
		}

		private static object GetState()
		{
#if UNITY_5_0
			return StateProperty.GetValue( AnimationWindow, null );
#else
			return StateField.GetValue( AnimEditor );
#endif
		}

		public static void SetCurrentFrame( int frame, float time )
		{
			if( AnimationWindow == null )
				return;

			object state = GetState();

#if UNITY_5_0
			TimeField.SetValue( state, time );
			FrameField.SetValue( state, frame );

			PreviewFrame.Invoke( AnimationWindow, new object[]{frame} );
#else
			CurrentTimeField.SetValue( state, time );
#endif

			_animationWindow.Repaint();
		}

		public static int GetCurrentFrame()
		{
			if( AnimationWindow == null )
				return -1;
#if UNITY_5_0
			return (int)FrameField.GetValue( GetState() );
#else
			return (int)FrameProperty.GetValue( GetState(), null );
#endif
		}

		public static void SelectAnimationClip( AnimationClip clip )
		{
			if( AnimationWindow == null || clip == null )
				return;

			AnimationClip[] clips = AnimationUtility.GetAnimationClips(Selection.activeGameObject);

			int index = 0;
			for( ; index != clips.Length; ++index )
			{
				if( clips[index] == clip )
					break;
			}


			if( index == clips.Length )
			{
				// didn't find
				Debug.LogError("Couldn't find clip " + clip.name);
			}
			else
			{
				// found
#if UNITY_5_0
				object[] selectedAnimation = (object[])SelectedAnimationField.GetValue( AnimationWindow );
				if( selectedAnimation.Length > 0 )
					ChooseClipMethod.Invoke( selectedAnimation[0], new object[]{clip} );
#else
				ActiveAnimationClipProperty.SetValue( GetState(), clip, null );
#endif
			}
		}
	}
}
