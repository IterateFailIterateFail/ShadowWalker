using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This doesn't work, whatever it is.
// update 2017-09-04: It works, but we should have probably namespaced it.

public class Debug
 {
     public static void Log(object obj)
     {
         UnityEngine.Debug.Log( System.DateTime.Now.ToLongTimeString() + " : " + obj );

     }

	public static void LogError(object message)
	{
		UnityEngine.Debug.LogError (System.DateTime.Now.ToLongTimeString () + " : " + message);
	}
 }
