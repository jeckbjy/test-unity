using UnityEngine;
using System.Collections;
using DG.Tweening;

public interface IAnim
{
	void Play();
	void Stop();
	float duration { get; }
}

public class AnimDoTween : IAnim
{
	private Tween _tween;
	private float _duration;

	public AnimDoTween(Tween tween)
	{
		this._tween = tween;
		_duration = tween.Delay() + tween.Duration();
	}

	public void Play()
	{
		_tween.Play();
	}

	public void Stop()
	{
		_tween.Kill();
	}

	public float duration {
		get {
			return _duration;
		}
	}
}
