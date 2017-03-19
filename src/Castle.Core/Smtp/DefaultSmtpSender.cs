// Copyright 2004-2009 Castle Project - http://www.castleproject.org/
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.


using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net;
using System.Net.Mail;
using System.Security.Permissions;
using Castle.Core.Core.Internal;

namespace Castle.Core.Core.Smtp
{
	public class DefaultSmtpSender : IEmailSender
	{
		private readonly NetworkCredential credentials = new NetworkCredential();
		private int? timeout;

		public DefaultSmtpSender()
		{
		}

		public DefaultSmtpSender(string hostname)
		{
			Hostname = hostname;
		}

		public int Port { get; set; } = 25;

		public string Hostname { get; }

		public bool AsyncSend { get; set; }

		public int Timeout
		{
			get { return timeout.HasValue ? timeout.Value : 0; }
			set { timeout = value; }
		}

		public bool UseSsl { get; set; }

		public string Domain
		{
			get { return credentials.Domain; }
			set { credentials.Domain = value; }
		}

		public string UserName
		{
			get { return credentials.UserName; }
			set { credentials.UserName = value; }
		}

		public string Password
		{
			get { return credentials.Password; }
			set { credentials.Password = value; }
		}

		private bool HasCredentials
		{
			get { return !string.IsNullOrEmpty(credentials.UserName); }
		}

		public void Send(string from, string to, string subject, string messageText)
		{
			if (from == null) throw new ArgumentNullException("from");
			if (to == null) throw new ArgumentNullException("to");
			if (subject == null) throw new ArgumentNullException("subject");
			if (messageText == null) throw new ArgumentNullException("messageText");

			Send(new MailMessage(from, to, subject, messageText));
		}

		public void Send(MailMessage message)
		{
			InternalSend(message);
		}

		public void Send(IEnumerable<MailMessage> messages)
		{
			foreach (var message in messages)
				Send(message);
		}

		private void InternalSend(MailMessage message)
		{
			if (message == null) throw new ArgumentNullException("message");

			if (AsyncSend)
			{
				// The MailMessage must be disposed after sending the email.
				// The code creates a delegate for deleting the mail and adds
				// it to the smtpClient.
				// After the mail is sent, the message is disposed and the
				// eventHandler removed from the smtpClient.

				var smtpClient = CreateSmtpClient();
				var msgGuid = new Guid();
				SendCompletedEventHandler sceh = null;
				sceh = delegate(object sender, AsyncCompletedEventArgs e)
				{
					if (msgGuid == (Guid) e.UserState)
						message.Dispose();
					// The handler itself, cannot be null, test omitted
					smtpClient.SendCompleted -= sceh;
				};
				smtpClient.SendCompleted += sceh;
				smtpClient.SendAsync(message, msgGuid);
			}
			else
			{
				using (message)
				{
					var smtpClient = CreateSmtpClient();

					smtpClient.Send(message);
				}
			}
		}

		protected virtual void Configure(SmtpClient smtpClient)
		{
			smtpClient.Credentials = null;

			if (CanAccessCredentials() && HasCredentials)
				smtpClient.Credentials = credentials;

			if (timeout.HasValue)
				smtpClient.Timeout = timeout.Value;

			if (UseSsl)
				smtpClient.EnableSsl = UseSsl;
		}

		private SmtpClient CreateSmtpClient()
		{
			if (string.IsNullOrEmpty(Hostname))
				return new SmtpClient();

			// A hostname is provided - init and configure using configured settings
			var smtpClient = new SmtpClient(Hostname, Port);
			Configure(smtpClient);
			return smtpClient;
		}

		private static bool CanAccessCredentials()
		{
			return new SecurityPermission(SecurityPermissionFlag.UnmanagedCode).IsGranted();
		}
	}
}