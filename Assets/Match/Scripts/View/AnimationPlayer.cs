using System;
using System.Collections.Generic;
using UnityEngine;

public class AnimationPlayer
{
	public delegate void FinishCallback();

	private float _animTime = 0;
	private List<IAnim>[] _anims;
	private bool _isRunning = false;
	private int _runningIndex = 0;
	private FinishCallback _onFinish;

	public bool isRunning {
		get {
			return _isRunning;
		}
	}

	public AnimationPlayer()
	{
		_anims = new List<IAnim>[2] {
				new List<IAnim> (), new List<IAnim> ()
			};
	}

	public void Update()
	{
		if (_animTime >= 0) {
			_animTime -= Time.deltaTime;
			if (_animTime < 0) {
				_isRunning = false;
				if (!Play()) {
					DoFinish();
				}
			}
		}
	}

	private int CurrentRunIndex()
	{
		return _runningIndex;
	}

	private int NextRunIndex()
	{
		return (_runningIndex + 1) % 2;
	}

	private void DoFinish()
	{
		if (_onFinish != null) {
			FinishCallback finish = _onFinish;
			_onFinish = null;
			finish();
		}
	}

	public int FindQueue()
	{
		if (_anims[0].Count == 0) {
			if (_anims[1].Count == 0) {
				return -1;
			} else {
				return 1;
			}
		} else {
			return 0;
		}
	}

	public bool Play()
	{
		if (_isRunning)
			return false;

		int index = FindQueue();
		if (index < 0) {
			DoFinish();
			return false;
		}

		RunIndex(index);
		return true;
	}

	public AnimationPlayer Wait(float time)
	{
		_animTime = time;
		return this;
	}

	public AnimationPlayer OnFinsh(FinishCallback finish)
	{
		_onFinish = finish;
		return this;
	}

	private void RunIndex(int index)
	{
		_animTime = 0.0001f;
		_runningIndex = index;
		foreach (var anim in _anims[index]) {
			_animTime = Math.Max(anim.duration, _animTime);
			anim.Play();
		}
		_anims[index].Clear();
		_isRunning = true;
	}

	public static AnimationPlayer operator +(AnimationPlayer player, IAnim anim)
	{
		if (anim == null) {
			return player;
		}

		if (player._isRunning) {
			player._anims[player.NextRunIndex()].Add(anim);
		} else {
			player._anims[player.CurrentRunIndex()].Add(anim);
		}
		return player;
	}

	//public static ZAnimationPlayer operator +(ZAnimationPlayer player, ZEraseMsgResponse msg)
	//{
	//	foreach (var anim in msg.anims) {
	//		player += anim;
	//	}
	//	return player;
	//}
}