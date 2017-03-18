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
		private bool asyncSend;
		private readonly string hostname;
		private int port = 25;
		private int? timeout;
		private bool useSsl;
		private readonly NetworkCredential credentials = new NetworkCredential();

		public DefaultSmtpSender() { }

		public DefaultSmtpSender(string hostname)
		{
			this.hostname = hostname;
		}

		public int Port
		{
			get { return port; }
			set { port = value; }
		}

		public string Hostname
		{
			get { return hostname; }
		}

		public bool AsyncSend
		{
			get { return asyncSend; }
			set { asyncSend = value; }
		}

		public int Timeout
		{
			get { return timeout.HasValue ? timeout.Value : 0; }
			set { timeout = value; }
		}

		public bool UseSsl
		{
			get { return useSsl; }
			set { useSsl = value; }
		}

		public void Send(String from, String to, String subject, String messageText)
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

		private void InternalSend(MailMessage message)
		{
			if (message == null) throw new ArgumentNullException("message");

			if (asyncSend)
			{
				// The MailMessage must be disposed after sending the email.
				// The code creates a delegate for deleting the mail and adds
				// it to the smtpClient.
				// After the mail is sent, the message is disposed and the
				// eventHandler removed from the smtpClient.

				SmtpClient smtpClient = CreateSmtpClient();
				Guid msgGuid = new Guid();
				SendCompletedEventHandler sceh = null;
				sceh = delegate(object sender, AsyncCompletedEventArgs e)
				{
					if (msgGuid == (Guid)e.UserState)
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
					SmtpClient smtpClient = CreateSmtpClient();

					smtpClient.Send(message);
				}
			}
		}

		public void Send(IEnumerable<MailMessage> messages)
		{
			foreach (MailMessage message in messages)
			{
				Send(message);
			}
		}

		public String Domain
		{
			get { return credentials.Domain; }
			set { credentials.Domain = value; }
		}

		public String UserName
		{
			get { return credentials.UserName; }
			set { credentials.UserName = value; }
		}

		public String Password
		{
			get { return credentials.Password; }
			set { credentials.Password = value; }
		}

		protected virtual void Configure(SmtpClient smtpClient)
		{
			smtpClient.Credentials = null;

			if (CanAccessCredentials() && HasCredentials)
			{
				smtpClient.Credentials = credentials;
			}

			if (timeout.HasValue)
			{
				smtpClient.Timeout = timeout.Value;
			}

			if (useSsl)
			{
				smtpClient.EnableSsl = useSsl;
			}
		}

		private bool HasCredentials
		{
			get { return !string.IsNullOrEmpty(credentials.UserName); }
		}

		private SmtpClient CreateSmtpClient()
		{
			if (string.IsNullOrEmpty(hostname))
			{
				// No hostname configured, use the settings provided in system.net.smtp (SmtpClient default behavior)
				return new SmtpClient();
			}

			// A hostname is provided - init and configure using configured settings
			var smtpClient = new SmtpClient(hostname, port);
			Configure(smtpClient);
			return smtpClient;
		}

		private static bool CanAccessCredentials()
		{
			return new SecurityPermission(SecurityPermissionFlag.UnmanagedCode).IsGranted();
		}
	}
}

