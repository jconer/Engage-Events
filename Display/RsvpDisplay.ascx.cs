// <copyright file="RsvpDisplay.ascx.cs" company="Engage Software">
// Engage: Events - http://www.engagemodules.com
// Copyright (c) 2004-2008
// by Engage Software ( http://www.engagesoftware.com )
// </copyright>
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
// TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
// THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF 
// CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
// DEALINGS IN THE SOFTWARE.

namespace Engage.Dnn.Events
{
    using System;
    using System.Diagnostics;
    using System.Globalization;
    using DotNetNuke.Services.Exceptions;
    using DotNetNuke.Services.Localization;
    using Engage.Events;

    /// <summary>
    /// Displays a summary of who has and hasn't RSVP'd for events.
    /// </summary>
    public partial class RsvpDisplay : ModuleBase
    {
        /// <summary>
        /// Backing field for <see cref="RsvpSummary"/>.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private Engage.Events.RsvpSummary rsvpSummary;

        /// <summary>
        /// Sets the RSVP summary to display.
        /// </summary>
        /// <param name="value">The RSVP summary to display.</param>
        public void SetRsvpSummary(Engage.Events.RsvpSummary value)
        {
            this.rsvpSummary = value;
        }

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Init"/> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> object that contains the event data.</param>
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            this.Load += this.Page_Load;
        }

        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!this.IsPostBack)
                {
                    this.TitleLabel.Text = this.rsvpSummary.Title;
                    this.NotAttendingLink.NavigateUrl = this.GetDetailUrl(this.rsvpSummary.EventId, RsvpStatus.NotAttending, this.rsvpSummary.NotAttending);
                    this.NotAttendingLink.Text = this.rsvpSummary.NotAttending.ToString(CultureInfo.CurrentCulture);
                    this.AttendingLink.NavigateUrl = this.GetDetailUrl(this.rsvpSummary.EventId, RsvpStatus.Attending, this.rsvpSummary.Attending);
                    this.AttendingLink.Text = this.rsvpSummary.Attending.ToString(CultureInfo.CurrentCulture);
                    this.DateLabel.Text = this.GetFormattedEventDate(this.rsvpSummary.EventStart, this.rsvpSummary.EventEnd);
                }
            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        /// <summary>
        /// Gets the formatted date string for this event.
        /// </summary>
        /// <param name="startDate">The event's start date.</param>
        /// <param name="endDate">The event's end date.</param>
        /// <returns>A formatted string representing the timespan over which this event occurs.</returns>
        private string GetFormattedEventDate(DateTime startDate, DateTime endDate)
        {
            return string.Format(CultureInfo.CurrentCulture, Localization.GetString("Timespan.Text", LocalResourceFile), startDate, endDate);
        }

        /// <summary>
        /// Gets the URL for the RSVP detail page of a given RSVP instance.
        /// </summary>
        /// <param name="eventId">The eventId.</param>
        /// <param name="status">The status.</param>
        /// <param name="count">The number of RSVP's for this event and status.</param>
        /// <returns>A URL for the RSVP detail page of a given RSVP instance</returns>
        private string GetDetailUrl(int eventId, RsvpStatus status, int count)
        {
            return count > 0
                       ? this.BuildLinkUrl(
                             "modId=" + this.ModuleId.ToString(CultureInfo.InvariantCulture),
                             "key=RsvpDetail",
                             "eventid=" + eventId.ToString(CultureInfo.InvariantCulture),
                             "status=" + status.ToString())
                       : string.Empty;
        }
    }
}