using UnityEngine;
using System.Collections;

namespace Flux
{
	[FEvent("Renderer/Set Texture", typeof(FRendererTrack))]
	public class FRendererTextureEvent : FRendererEvent {

		[SerializeField]
		private Texture _texture = null;

		protected override void ApplyProperty( float t )
		{
#if UNITY_EDITOR
			if( _texture != null )
#endif
				_matPropertyBlock.SetTexture( PropertyName, _texture );
		}
	}
}
