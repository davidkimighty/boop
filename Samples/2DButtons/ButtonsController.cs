using boop;
using UnityEngine;
using UnityEngine.EventSystems;

namespace boop_sample
{
    public class ButtonsController : MonoBehaviour
    {
        [SerializeField] private Button _firstButton;
        
        private void Awake()
        {
            EventSystem.current.SetSelectedGameObject(_firstButton.gameObject);
        }
    }
}
