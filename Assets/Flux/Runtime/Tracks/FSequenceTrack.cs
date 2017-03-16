using UnityEngine;
using System.Collections.Generic;

namespace Flux
{
	public class FSequenceTrack : FTrack
	{
		private FSequence _ownerSequence = null;
		private FSequence OwnerSequence { 
			get { 
				if( _ownerSequence == null )
					_ownerSequence = Owner.GetComponent<FSequence>();
				return _ownerSequence;
			}
		}
		public override bool PreviewDirtiesScene { 
			get {
				if( OwnerSequence != null )
				{
					foreach( FContainer container in OwnerSequence.Containers )
					{
						foreach( FTimeline timeline in container.Timelines )
						{
							foreach( FTrack track in timeline.Tracks )
							{
								if( track.PreviewDirtiesScene )
									return true;
							}
						}
					}
				}

				return false; 
			} 
		}
		public override bool CanTogglePreview { get { return true; } }

		public override CacheMode RequiredCacheMode {
			get {
				return CacheMode.Editor | CacheMode.RuntimeBackwards;
			}
		}

		public override CacheMode AllowedCacheMode {
			get {
				return RequiredCacheMode | CacheMode.RuntimeForward;
			}
		}

//		public override bool RequiresEditorCache()
//		{
//			return true;
//		}

		public override void CreateCache()
		{
			if( HasCache )
				return;

			Cache = new FSequenceTrackCache( this );
			Cache.Build();
		}

		public override void ClearCache()
		{
			if( !HasCache )
				return;

			Cache.Clear();
			Cache = null;
		}

		public override void Init ()
		{
			base.Init();

			if( Application.isPlaying )
				enabled = true;
		}
	}
}
