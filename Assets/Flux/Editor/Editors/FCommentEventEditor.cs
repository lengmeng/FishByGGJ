using UnityEngine;
using UnityEditor;

using Flux;

namespace FluxEditor
{

	[FEditor(typeof(FCommentEvent))]
	public class FCommentEventEditor : FEventEditor {

		private GUIStyle _textStyle = null;

//		protected override void RenderEvent (FrameRange viewRange, FrameRange validKeyframeRange)
//		{
//			base.RenderEvent (viewRange, validKeyframeRange);
//
//			if( Event.current.type == EventType.Repaint )
//			{
//				if( viewRange.Overlaps( Evt.FrameRange ) )
//				{
//					if( _textStyle == null )
//						InitTextStyle();
//					_textStyle.Draw( _eventRect, new GUIContent(((FCommentEvent)Evt).Comment), 0 );
////					GUI.Label(_eventRect,  );
//				}
//			}
//		}

		private void InitTextStyle()
		{
			_textStyle = new GUIStyle(EditorStyles.whiteLabel);
			
			_textStyle.padding.left = 5;
			_textStyle.padding.right = 5;
			
			_textStyle.alignment = TextAnchor.MiddleCenter;
		}

		public override Color GetColor ()
		{
			return ((FCommentEvent)Evt).Color;
		}

		public override GUIStyle GetEventStyle()
		{
			return FUtility.GetCommentEventStyle();
		}

		public override GUIStyle GetTextStyle()
		{
			if( _textStyle == null ) InitTextStyle();
			return _textStyle;
		}
	}
}
