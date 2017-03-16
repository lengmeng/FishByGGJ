using UnityEngine;

namespace Flux
{
	[FEventAttribute("Misc/Comment", typeof(FCommentTrack))]
	public class FCommentEvent : FEvent {

		[SerializeField]
		private string _comment = "!Comment!";
//		public string Comment { get { return _comment; } set { _comment = value; } }
		public override string Text {
			get {
				return _comment;
			}
			set {
				_comment = value;
			}
		}

		[SerializeField]
		private Color _color = new Color( 0.15f, 0.6f, 0.95f, 0.8f );
		public Color Color { get { return _color; } set { _color = value; } }

	}
}