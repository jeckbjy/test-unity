using UnityEngine;
using System.Collections;

//public class Singleton<T> : ZComponentBase where T : Singleton<T>
//{
//	private static T _instance;
//	private static bool _isDestroyed = false;
//	public static T Instance {
//		get {
//			if (_isDestroyed)
//				return null;
//			if (_instance == null) {
//				GameObject singleton = new GameObject();
//				_instance = singleton.AddComponent<T>();
//				singleton.name = typeof(T).ToString();
//			}
//			return _instance;
//		}
//	}

//	public Singleton() : base()
//	{
//		//if (_instance != null)
//			//Debug.LogError("Singleton " + typeof(T).GetType() + " duplicated");
//		_instance = this as T;
//	}

//	protected virtual void OnDestroy()
//	{
//		_isDestroyed = true;
//	}
//}

public class Singleton<T> where T : class, new() {
	private static T _instance;
	public static T Instance {
		get {
			if (_instance == null) {
				_instance = new T();
			}

			return _instance;
		}
	}
}