using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace EasyFeedback
{
    public class ReportSection
    {
        private string title;
        private StringBuilder sectionText;

        public string Title
        {
            get { return title; }
        }

        /// <summary>
        /// The order of this element in the report (lowest first)
        /// </summary>
        public int SortOrder;

        /// <summary>
        /// Creates a new report section with the specified title and sort order
        /// </summary>
        /// <param name="title"></param>
        /// <param name="sortOrder"></param>
        public ReportSection(string title, int sortOrder = 0)
        {
            this.title = title;
            SortOrder = sortOrder;
            sectionText = new StringBuilder();
        }

        /// <summary>
        /// Creates a new report section with the specified title and text
        /// </summary>
        /// <param name="title"></param>
        /// <param name="text"></param>
        public ReportSection(string title, string text)
        {
            this.title = title;
            sectionText = new StringBuilder(text);
        }

        /// <summary>
        /// Appends text to the section text
        /// </summary>
        /// <param name="text"></param>
        public void Append(string text)
        {
            sectionText.Append(text);
        }

        /// <summary>
        /// Appends a line to the section text
        /// </summary>
        /// <param name="line"></param>
        public void AppendLine(string line)
        {
            sectionText.AppendLine(line);
        }

        /// <summary>
        /// Replaces the existing section text with specified text
        /// </summary>
        /// <param name="text"></param>
        public void SetText(string text)
        {
            this.sectionText = new StringBuilder(text);
        }

        /// <summary>
        /// Returns the section in markdown formatting for Trello
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            StringBuilder section = new StringBuilder();
            
            section.AppendLine(Markdown.H3(title));

            // append section text
            section.AppendLine(sectionText.ToString());

            return section.ToString();
        }
    }
}