using UnityEngine;
using System;
using System.Reflection;

namespace Flux
{
	[FEvent("Script/Tween Vector2")]
	public class FTweenVector2Event : FTweenVariableEvent<FTweenVector2>  {

		protected override object GetValueAt( float t )
		{
			return _tween.GetValue( t );
		}
	}
}
