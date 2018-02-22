using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace EasyFeedback
{
    class GraphicsInfoCollector : FormField
    {

        public override void FormClosed()
        {
        }

        public override void FormOpened()
        {
        }

        public override void FormSubmitted()
        {
            // add section to report if it doesn't already exist
            if (!Form.CurrentReport.HasSection(SectionTitle))
                Form.CurrentReport.AddSection(SectionTitle, SortOrder);

            // append graphics info to section
            Form.CurrentReport["Additional Info"].AppendLine("Quality Level: " + QualitySettings.names[QualitySettings.GetQualityLevel()]);
            Form.CurrentReport["Additional Info"].AppendLine("Resolution: " + Screen.width + "x" + Screen.height);
            Form.CurrentReport["Additional Info"].AppendLine("Full Screen: " + Screen.fullScreen);
        }
    }

}