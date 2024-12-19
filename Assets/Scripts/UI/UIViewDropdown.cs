using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UI
{
    public class UIViewDropdown : MonoBehaviour
    {
        private TMP_Dropdown _dropdown;
        private List<string> _displayNames = new List<string>();
        private ViewManager _viewManager;
        void Awake()
        {
            _dropdown = GetComponent<TMP_Dropdown>();
            
            //oops singleton pattern
            _viewManager = GameObject.FindFirstObjectByType<ViewManager>();//cache
            _dropdown.onValueChanged.AddListener(OnChange);
            
        }

        void InitSceneList()
        {
            if (_viewManager == null)
            {
                return;
            }
            //indices here will align
            _displayNames = _viewManager.Views.Select(x => x.DisplayName).ToList();
            
            _dropdown.ClearOptions();
            _dropdown.AddOptions(_displayNames);
        }

        private void OnChange(int newCurrent)
        {
            _viewManager.SwitchView(newCurrent);
        }
        private void OnEnable()
        {
            InitSceneList();
            var current = GameSetings.ViewScene;
            _dropdown.SetValueWithoutNotify(current);
        }
    }
}