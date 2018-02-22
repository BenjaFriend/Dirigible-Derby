using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace EasyFeedback
{
    class PlayerInfoCollector : FormField
    {
        public override void FormClosed()
        {
        }

        public override void FormOpened()
        {
        }

        public override void FormSubmitted()
        {
            // include player position if player exists
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player == null)
                return;

            // add section to report if it doesn't already exist
            if (!Form.CurrentReport.HasSection(SectionTitle))
                Form.CurrentReport.AddSection(SectionTitle, SortOrder);

            // add player info to report
            Form.CurrentReport["Additional Info"].AppendLine("Player Position: " + player.transform.position.ToString());
        }
    }

}