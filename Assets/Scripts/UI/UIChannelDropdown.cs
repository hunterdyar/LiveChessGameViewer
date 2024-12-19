using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

namespace UI
{
    public class UIChannelDropdown : MonoBehaviour
    {
        private TMP_Dropdown _dropdown;
        private List<string> _options;
        private void Awake()
        {
            _dropdown = GetComponent<TMP_Dropdown>();
            _dropdown.onValueChanged.AddListener(OnChanged);
        }

        private void OnChanged(int index)
        {
           var cname = _options[index].ToLower();
           GameSetings.SetChannel(cname);
        }
        

        private void OnEnable()
        {
            _dropdown.ClearOptions();
            //turns the string array into this array with capital each word.
            var current = ChessLiveViewManager.ChannelList.IndexOf(GameSetings.ChessChannel);
            if (current < 0)
            {
                Debug.LogWarning("uh oh, our channel isn't saved correctly.");
                current = 0;
            }
            
            _options = ChessLiveViewManager.ChannelList.channels
                .Select(x => char.ToUpper(x.ChannelName[0]) + x.ChannelName.Remove(0, 1)).ToList();
            _dropdown.AddOptions(_options);
            _dropdown.SetValueWithoutNotify(current);
        }
    }
}