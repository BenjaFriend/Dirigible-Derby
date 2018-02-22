using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace EasyFeedback
{
    class SystemInfoCollector : FormField
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

            // append system info to section
            Form.CurrentReport["System Info"].AppendLine("OS: " + SystemInfo.operatingSystem);
            Form.CurrentReport["System Info"].AppendLine("Processor: " + SystemInfo.processorType);
            Form.CurrentReport["System Info"].AppendLine("Memory: " + SystemInfo.systemMemorySize);
            Form.CurrentReport["System Info"].AppendLine("Graphics API: " + SystemInfo.graphicsDeviceType);
            Form.CurrentReport["System Info"].AppendLine("Graphics Processor: " + SystemInfo.graphicsDeviceName);
            Form.CurrentReport["System Info"].AppendLine("Graphics Memory: " + SystemInfo.graphicsMemorySize);
            Form.CurrentReport["System Info"].AppendLine("Graphics Vendor: " + SystemInfo.graphicsDeviceVendor);
        }
    }

}