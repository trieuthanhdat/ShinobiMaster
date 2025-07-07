//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using Facebook.Unity;

//public class FaceBookInit : MonoBehaviour
//{

//    private void Awake()
//    {
//        if (FB.IsInitialized)
//        {
//            FB.ActivateApp();
//        }
//        else {
//            FB.Init(FB.ActivateApp);
//        }
//    }

//    private void OnApplicationPause(bool pause)
//    {
//        if (!pause) {
//            if (FB.IsInitialized)
//            {
//                FB.ActivateApp();
//            }
//            else {
//                FB.Init(FB.ActivateApp);
//            }
//        }
//    }
//}
