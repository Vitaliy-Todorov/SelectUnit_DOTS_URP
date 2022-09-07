using Assets.Scripts.Services.InputServices;
using UnityEngine;

namespace Assets.Scripts.Services
{
    public class RegisterServices : MonoBehaviour
    {
        private void Awake()
        {
            AllServices.Container.RegisterSingle(new InputKeyboardMouseService());
        }
    }
}