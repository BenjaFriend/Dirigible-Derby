using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace EasyFeedback
{
    [RequireComponent(typeof(Dropdown))]
    public class CategoryDropdown : FormElement
    {
        private Dropdown typeDropdown;

        public override void Awake()
        {
            base.Awake();
            typeDropdown = GetComponent<Dropdown>();

            // add options
            typeDropdown.ClearOptions();
            for (int i = 0; i < Form.Config.Board.CategoryNames.Length; i++)
            {
                typeDropdown.options.Add(new Dropdown.OptionData(Form.Config.Board.CategoryNames[i]));
            }
            typeDropdown.value = 0;
            typeDropdown.RefreshShownValue();
        }

        public override void FormClosed()
        {
        }

        public override void FormOpened()
        {
        }

        public override void FormSubmitted()
        {
            Form.CurrentReport.List.id = Form.Config.Board.CategoryIds[typeDropdown.value];
            Form.CurrentReport.List.name = Form.Config.Board.CategoryNames[typeDropdown.value];
        }
    }
}