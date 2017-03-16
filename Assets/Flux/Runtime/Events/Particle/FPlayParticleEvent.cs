using UnityEngine;
using System.Collections;

namespace Flux
{
	[FEvent( "Particle System/Play Particle" )]
	public class FPlayParticleEvent : FEvent {

		[SerializeField]
		[Tooltip("True: ParticleSystem playback speed will be adjusted to match event length"
		         +"\nFalse: ParticleSystem plays at normal speed, i.e. doesn't scale based on event length")]
		private bool _normalizeToEventLength = false;

		private ParticleSystem _particleSystem = null;

		protected override void OnInit ()
		{
			_particleSystem = Owner.GetComponent<ParticleSystem>();
#if UNITY_EDITOR
			if( _particleSystem == null )
				Debug.LogError("FParticleEvent is attached to an object that doesn't have a ParticleSystem");
#endif
		}

		protected override void OnTrigger( float timeSinceTrigger )
		{
			if( _particleSystem != null )
			{
				_particleSystem.Play( true );
			}
		}

		protected override void OnFinish()
		{
			if( _particleSystem != null )
				_particleSystem.Stop( true );

		}

		protected override void OnStop()
		{
			if( _particleSystem != null )
				_particleSystem.Clear( true );
		}

		protected override void OnUpdateEvent( float timeSinceTrigger )
		{
			if( !Sequence.IsPlaying || !Sequence.IsPlayingForward )
			{
				float t = _normalizeToEventLength ? (timeSinceTrigger / LengthTime) * _particleSystem.duration : timeSinceTrigger;
				_particleSystem.Simulate( t, true, true );
			}
		}

//        protected override void OnUpdateEventEditor( float timeSinceTrigger )
//		{
//			float t = _normalizeToEventLength ? timeSinceTrigger / LengthTime : timeSinceTrigger;
//
//			if( _particleSystem != null )
//				_particleSystem.Simulate( t, true, true );
//		}

	}
}
