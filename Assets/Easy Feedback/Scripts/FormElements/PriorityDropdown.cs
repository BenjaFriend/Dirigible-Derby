using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace EasyFeedback
{
    [RequireComponent(typeof(Dropdown))]
    public class PriorityDropdown : FormElement
    {
        private Dropdown priorityDropdown;

        public override void Awake()
        {
            base.Awake();
            priorityDropdown = GetComponent<Dropdown>();

            // add options
            priorityDropdown.ClearOptions();
            for (int i = 0; i < Form.Config.Board.Labels.Length; i++)
            {
                priorityDropdown.options.Add(new Dropdown.OptionData(Form.Config.Board.Labels[i].name));
            }
            priorityDropdown.value = 0;
            priorityDropdown.RefreshShownValue();
        }

        public override void FormClosed()
        {
        }

        public override void FormOpened()
        {
        }

        public override void FormSubmitted()
        {
            Form.CurrentReport.Label = Form.Config.Board.Labels[priorityDropdown.value];
        }
    }
}
