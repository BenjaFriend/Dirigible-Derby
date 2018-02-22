using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace EasyFeedback
{
    public class Report
    {
        static int MAX_ATTACHMENTS = 99; // Trello limit is 100 attachments per card, screenshot counts as one

        /// <summary>
        /// Report information stored by title
        /// </summary>
        private Dictionary<string, ReportSection> info;

        /// <summary>
        /// Trello list this report will be added to 
        /// </summary>
        public APIs.List List;

        /// <summary>
        /// Labels to add to the card on Trello
        /// </summary>
        public APIs.Label Label;

        /// <summary>
        /// The title of the card on Trello
        /// </summary>
        public string Title;

        /// <summary>
        /// Binary data for screenshot to be included with this report
        /// </summary>
        public byte[] Screenshot;

        /// <summary>
        /// Additional files attached to this report
        /// </summary>
        /// <remarks>
        /// Private to enforce Trello attachment limit (100)
        /// </remarks>
        private List<FileAttachment> attachments;

        public List<FileAttachment> Attachments
        {
            get { return attachments; }
        }


        public Report()
        {
            // initialize info collection
            info = new Dictionary<string, ReportSection>();

            // initalize attachments list
            attachments = new List<FileAttachment>();
        }

        /// <summary>
        /// Adds a new empty section to the report
        /// </summary>
        /// <param name="title">The title of the section</param>
        /// <param name="sortOrder">The order of the section on the report (lowest first)</param>
        public void AddSection(string title, int sortOrder = 0)
        {
            AddSection(new ReportSection(title, sortOrder));
        }

        /// <summary>
        /// Adds a new section to the report
        /// </summary>
        /// <param name="section"></param>
        public void AddSection(ReportSection section)
        {
            if (info.ContainsKey(section.Title))
            {
                // Do we want to eventually support multiple sections sharing the same title?
                Debug.LogError("Report already contains a section with title \"" + section.Title + "\"");
                return;
            }

            // add the section to the dictionary
            info.Add(section.Title, section);
        }

        public void RemoveSection(string title)
        {
            if (!info.ContainsKey(title))
            {
                Debug.LogWarning("Can not remove section \"" + title + "\" because report does not contain a section with that name");
                return;
            }

            info.Remove(title);
        }

        /// <summary>
        /// Checks whether the report already has a section
        /// </summary>
        /// <param name="title"></param>
        /// <returns></returns>
        public bool HasSection(string title)
        {
            return info.ContainsKey(title);
        }

        /// <summary>
        /// Returns the report formatted in markdown for Trello
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            StringBuilder report = new StringBuilder();

            // sort report sections
            ReportSection[] sections = info.Select(r => r.Value)
                                            .OrderBy(v => v.SortOrder).ToArray();

            // build report string
            for (int i = 0; i < sections.Length; i++)
            {
                report.AppendLine(sections[i].ToString());
            }

            return report.ToString();
        }

        public string GetLocalFileText()
        {
            StringBuilder report = new StringBuilder();

            // add category and label
            report.AppendLine(Markdown.H3("Category"));
            report.AppendLine(List.name);
            report.AppendLine();

            report.AppendLine(Markdown.H3("Label"));
            report.AppendLine(Label.name);
            report.AppendLine();

            // add the rest of the report
            report.AppendLine(this.ToString());

            return report.ToString();
        }

        /// <summary>
        /// Attach a file to the report
        /// </summary>
        /// <param name="file"></param>
        public void AttachFile(FileAttachment file)
        {

            if(attachments.Count + 1 > MAX_ATTACHMENTS)
            {
                Debug.LogError("Error attaching file: maximum attachment limit (" + MAX_ATTACHMENTS + ") reached!");
                return;
            }

            attachments.Add(file);
        }

        /// <summary>
        /// Attach a file to the report
        /// </summary>
        /// <param name="name">The name of the file</param>
        /// <param name="filePath">The path to the file</param>
        public void AttachFile(string name, string filePath)
        {
            AttachFile(new FileAttachment(name, filePath, null));
        }

        /// <summary>
        /// Attach a file to the report
        /// </summary>
        /// <param name="name">The name of the file</param>
        /// <param name="data">The file data</param>
        public void AttachFile(string name, byte[] data)
        {
            AttachFile(new FileAttachment(name, data));
        }

        /// <summary>
        /// Returns a section in the report by title
        /// </summary>
        /// <param name="sectionTitle"></param>
        /// <returns></returns>
        public ReportSection this[string sectionTitle]
        {
            get
            {
                if (info.ContainsKey(sectionTitle))
                {
                    return info[sectionTitle];
                }
                else
                {
                    Debug.LogError("Report does not contain a section with title \"" + sectionTitle + "\"");
                    return null;
                }
            }
            set
            {
                if (info.ContainsKey(sectionTitle))
                {
                    info[sectionTitle] = value;
                }
                else
                {
                    Debug.LogError("Report does not contain a section with title \"" + sectionTitle + "\"");
                }
            }
        }
    }
}
