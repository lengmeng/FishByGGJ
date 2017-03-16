using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

using Flux;

namespace FluxEditor
{
	[FEditor(typeof(FPlayAudioEvent))]
	public class FPlayAudioEventEditor : FEventEditor {

		private Texture2D _waveformTexture = null;
		private FPlayAudioEvent _playAudioEvt = null;
		private AudioClip _currentClip = null;

		private SerializedProperty _startOffset;

		private int _frameMouseDown = int.MaxValue;

		public override void Init( FObject obj, FEditor owner )
		{
			base.Init( obj, owner);
			_playAudioEvt = (FPlayAudioEvent)obj;

			_startOffset = new SerializedObject( _playAudioEvt ).FindProperty( "_startOffset" );
		}

		protected override void RenderEvent( FrameRange viewRange, FrameRange validKeyframeRange )
		{
			int startOffsetHandleId = EditorGUIUtility.GetControlID(FocusType.Passive);

			switch( Event.current.type )
			{
			case EventType.MouseDown:
				if( Event.current.alt && EditorGUIUtility.hotControl == 0 && _eventRect.Contains(Event.current.mousePosition) )
				{
					EditorGUIUtility.hotControl = startOffsetHandleId;
					_frameMouseDown = SequenceEditor.GetFrameForX( Event.current.mousePosition.x ) - _playAudioEvt.Start;
					Event.current.Use();
				}
				break;

			case EventType.MouseDrag:
				if( EditorGUIUtility.hotControl == startOffsetHandleId )
				{
					int mouseCurrentFrame = Mathf.Clamp( SequenceEditor.GetFrameForX( Event.current.mousePosition.x ) - _playAudioEvt.Start, 0, _playAudioEvt.Length );

					int mouseDelta =  _frameMouseDown - mouseCurrentFrame;

					if( mouseDelta != 0 )
					{
						_frameMouseDown = mouseCurrentFrame;
						_startOffset.serializedObject.Update();
						_startOffset.intValue = Mathf.Clamp( _startOffset.intValue + mouseDelta, 0, _playAudioEvt.GetMaxStartOffset() );
						_startOffset.serializedObject.ApplyModifiedProperties();
					}
					Event.current.Use();
				}
				break;
			case EventType.MouseUp:
				if( EditorGUIUtility.hotControl == startOffsetHandleId )
				{
					EditorGUIUtility.hotControl = 0;
					Event.current.Use();
				}
				break;
			}

			int frameRangeLength = _playAudioEvt.Length;

			base.RenderEvent( viewRange, validKeyframeRange );

			if( frameRangeLength != _playAudioEvt.Length )
			{
				_startOffset.serializedObject.Update();
				_startOffset.intValue = Mathf.Clamp( _startOffset.intValue, 0, _playAudioEvt.GetMaxStartOffset() );
				_startOffset.serializedObject.ApplyModifiedProperties();
			}

			if( _currentClip != _playAudioEvt.AudioClip )
			{
				_waveformTexture = FUtility.GetAudioClipTexture(_playAudioEvt.AudioClip, Rect.width, 64 );
				_currentClip = _playAudioEvt.AudioClip;
			}

			if( Event.current.type == EventType.Repaint && _waveformTexture != null )
			{
				float border = 2;

				Rect waveformRect = _eventRect;
				waveformRect.xMin += border;
				waveformRect.xMax -= border;

				FrameRange visibleEvtRange = _playAudioEvt.FrameRange;
				float startOffset = _playAudioEvt.StartOffset;
				if( viewRange.Start > visibleEvtRange.Start )
				{
					startOffset += viewRange.Start - visibleEvtRange.Start;
					visibleEvtRange.Start = viewRange.Start;
				}

				if( viewRange.End < visibleEvtRange.End )
				{
					visibleEvtRange.End = viewRange.End;
				}

				float uvLength = (visibleEvtRange.Length*_playAudioEvt.Sequence.InverseFrameRate) / _playAudioEvt.AudioClip.length;

				Rect waveformUVRect = new Rect( (startOffset / _playAudioEvt.FrameRange.Length)*((_playAudioEvt.FrameRange.Length*_playAudioEvt.Sequence.InverseFrameRate) / _playAudioEvt.AudioClip.length), 0, uvLength, 1f );
				float uvPerPixel = uvLength / waveformRect.width;

				float borderUVs = border * uvPerPixel;

				waveformUVRect.xMin += borderUVs;
				waveformUVRect.xMax -= borderUVs;

				if( _currentClip.channels == 1 )
				{
					waveformUVRect.yMin = 0.3f;
					waveformUVRect.yMax = 0.7f;
				}
				GUI.DrawTextureWithTexCoords( waveformRect, _waveformTexture, waveformUVRect );
			}
		}
	}
}
