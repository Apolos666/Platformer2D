using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Askeladd.Scripts.GeneralScripts
{
    public static class CommonFunctions 
    {
        public static void ClearArray(System.Array array, int index, int arrayLength)
        {
            System.Array.Clear(array, index, arrayLength);
        }
    }
}
