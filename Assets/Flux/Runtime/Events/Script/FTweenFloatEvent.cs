using UnityEngine;
using System;
using System.Reflection;

namespace Flux
{
	[FEvent("Script/Tween Float")]
	public class FTweenFloatEvent : FTweenVariableEvent<FTweenFloat>  {

		protected override object GetValueAt( float t )
		{
			return _tween.GetValue( t );
		}
	}
}
