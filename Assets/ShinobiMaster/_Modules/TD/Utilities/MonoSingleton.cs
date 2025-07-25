﻿/*
 *	The MIT License (MIT)
 *
 *	Copyright (c) 2014 Mateusz Majchrzak
 *
 *	Permission is hereby granted, free of charge, to any person obtaining a copy
 *	of this software and associated documentation files (the "Software"), to deal
 *	in the Software without restriction, including without limitation the rights
 *	to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 *	copies of the Software, and to permit persons to whom the Software is
 *	furnished to do so, subject to the following conditions:
 *
 *	The above copyright notice and this permission notice shall be included in all
 *	copies or substantial portions of the Software.
 *
 *	THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 *	IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 *	FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 *	AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 *	LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 *	OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 *	SOFTWARE.
 */

using UnityEngine; 

/// <summary>
/// 
/// </summary>
public class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour {
    /// <summary>
    /// 
    /// </summary>
    private static T s_Instance;

    public static bool s_IsCreated = false;

    /// <summary>
    /// singleton property
    /// </summary>
    public static T Instance
    {
        get
        {
            if (s_IsCreated)
                return s_Instance;

            if (s_Instance == null) {
                s_Instance = GameObject.FindObjectOfType(typeof(T)) as T;

                if (s_Instance == null) {
                    GameObject gameObject = new GameObject(typeof(T).Name);
                    s_Instance = gameObject.AddComponent(typeof(T)) as T;
                }

                s_IsCreated = true;
            }

            return s_Instance;
        }
    }
	public static T instance
	{
		get
		{
			if (!Application.isPlaying) {
				return null;
			}
			if (s_IsCreated)
				return s_Instance;

			if (s_Instance == null) {
				s_Instance = GameObject.FindObjectOfType(typeof(T)) as T;

				if (s_Instance == null) {
					GameObject gameObject = new GameObject(typeof(T).Name);

					s_Instance = gameObject.AddComponent(typeof(T)) as T;
				}

				s_IsCreated = true;
			}

			return s_Instance;
		}
	}
    /// <summary>
    /// 
    /// </summary>
    protected virtual void OnDestroy() {
        if (s_Instance != null && s_Instance.gameObject == gameObject)
        {
            Destroy(s_Instance);

            s_Instance = null;
            s_IsCreated = false;
        }
    }

    protected virtual void OnApplicationPause(bool isPause)
    {
        
    }
}