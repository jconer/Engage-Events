﻿// <copyright file="EventToolTip.ascx.cs" company="Engage Software">
// Engage: Events - http://www.EngageSoftware.com
// Copyright (c) 2004-2011
// by Engage Software ( http://www.engagesoftware.com )
// </copyright>
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
// TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
// THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF 
// CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
// DEALINGS IN THE SOFTWARE.

namespace Engage.Dnn.Events.Display
{
    using System;
    using System.Diagnostics;
    using System.Globalization;
    using System.Web;
    using System.Web.UI;
    using DotNetNuke.Framework;
    using Engage.Events;

    /// <summary>
    /// Used to display a "tool tip" for an appointment.
    /// </summary>
    public partial class EventToolTip : ModuleBase
    {
        /// <summary>
        /// The backing field for <see cref="SetEvent"/>.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private Event currentEvent;

        /// <summary>
        /// Sets the event to be displayed in the event tooltip.
        /// </summary>
        /// <param name="tooltipEvent">The event to display in the tooltip.</param>
        public void SetEvent(Event tooltipEvent)
        {
            this.currentEvent = tooltipEvent;
        }

        /// <summary>
        /// Shows the event.
        /// </summary>
        public void ShowEvent()
        {
            this.EventDate.Text = Dnn.Events.Utility.GetFormattedEventDate(this.currentEvent.EventStart, this.currentEvent.EventEnd, this.LocalResourceFile);
            
            var userTimeZone = Dnn.Utility.GetUserTimeZone();
            this.EventTimeZone.Text = HttpUtility.HtmlEncode(
                string.Format(
                    CultureInfo.CurrentCulture,
                    this.Localize(this.currentEvent.TimeZone.Equals(userTimeZone) ? "SameTimeZone.Format" : "DifferentTimeZone.Format"),
                    this.currentEvent.UserEventStart,
                    this.currentEvent.UserEventEnd,
                    userTimeZone.IsDaylightSavingTime(this.currentEvent.UserEventStart) ? userTimeZone.DaylightName : userTimeZone.StandardName,
                    userTimeZone.IsDaylightSavingTime(this.currentEvent.UserEventEnd) ? userTimeZone.DaylightName : userTimeZone.StandardName,
                    userTimeZone.DisplayName,
                    userTimeZone.BaseUtcOffset.TotalHours,
                    userTimeZone.BaseUtcOffset.Minutes,
                    this.currentEvent.TimeZone.IsDaylightSavingTime(this.currentEvent.EventStart) ? this.currentEvent.TimeZone.DaylightName : this.currentEvent.TimeZone.StandardName,
                    this.currentEvent.TimeZone.IsDaylightSavingTime(this.currentEvent.EventEnd) ? this.currentEvent.TimeZone.DaylightName : this.currentEvent.TimeZone.StandardName,
                    this.currentEvent.TimeZone.DisplayName,
                    this.currentEvent.TimeZone.BaseUtcOffset.TotalHours,
                    this.currentEvent.TimeZone.BaseUtcOffset.Minutes));

            this.EventOverview.Text = this.currentEvent.Overview;
            this.EventTitle.Text = this.currentEvent.Title;
            this.EventLink.NavigateUrl = this.BuildLinkUrl(this.DetailsTabId, this.DetailsModuleId, "EventDetail", Dnn.Events.Utility.GetEventParameters(this.currentEvent));

            this.RegisterButton.Visible = this.currentEvent.AllowRegistrations;
        }

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Init"/> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> object that contains the event data.</param>
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            this.PreRender += this.Page_PreRender;
            this.AddToCalendarButton.Click += this.AddToCalendarButton_Click;
            this.EditButton.Click += this.EditButton_Click;

            this.RegisterButton.CurrentEvent = this.currentEvent;
            this.RegisterButton.ModuleConfiguration = this.ModuleConfiguration;
            this.RegisterButton.LocalResourceFile = this.LocalResourceFile;

            AJAX.RegisterPostBackControl(this.AddToCalendarButton);
        }

        /// <summary>
        /// Handles the <see cref="Control.PreRender"/> event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void Page_PreRender(object sender, EventArgs e)
        {
            ////this.ShowEvent();
            ////this.AddToCalendarButton.Visible = Engage.Utility.IsLoggedIn;
            this.EditButton.Visible = this.IsEditable || this.PermissionsService.CanManageEvents;
        }

        /// <summary>
        /// Handles the Click event of the AddToCalendarButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void AddToCalendarButton_Click(object sender, EventArgs e)
        {
            ModuleBase.SendICalendarToClient(this.Response, this.currentEvent.ToICal(), this.currentEvent.Title);
        }

        /// <summary>
        /// Handles the Click event of the EditButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void EditButton_Click(object sender, EventArgs e)
        {
            this.Response.Redirect(this.BuildLinkUrl(this.ModuleId, "EventEdit", Dnn.Events.Utility.GetEventParameters(this.currentEvent)), true);
        }
    }
}