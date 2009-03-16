﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DotNetOpenAuth.OpenId.RelyingParty;
using DotNetOpenAuth.OpenId.Messages;
using DotNetOpenAuth.OpenId;
using DotNetOpenAuth.Messaging;

public partial class OP_AssociateHttpNoEncryption : System.Web.UI.Page {
	OpenIdRelyingParty rp = new OpenIdRelyingParty();

	protected void Page_Load(object sender, EventArgs e) {
		if (!IsPostBack) {
			identifierBox.Focus();
		}
	}
	protected void beginButton_Click(object sender, EventArgs e) {
		if (!Page.IsValid) {
			return;
		}

		Uri providerEndpoint;
		Version providerVersion;
		DiscoverHttpEndpoint(identifierBox.Text, out providerEndpoint, out providerVersion);
		if (providerEndpoint == null) {
			this.errorLabel.Text = "No HTTP provider endpoint found.";
			this.errorLabel.Visible = true;
		} else {
			Protocol protocol = Protocol.Lookup(providerVersion);
			testResultDisplay.ProviderEndpoint = providerEndpoint;
			testResultDisplay.ProtocolVersion = providerVersion;
			var associate = new AssociateUnencryptedRequestNoCheck(providerVersion, providerEndpoint) {
				AssociationType = protocol.Args.SignatureAlgorithm.HMAC_SHA1,
			};

			try {
				var response = rp.Channel.Request<DirectErrorResponse>(associate);
				testResultDisplay.Pass = true;
				testResultDisplay.Details = response.ErrorMessage;
			} catch (ProtocolException ex) {
				testResultDisplay.Pass = false;
				testResultDisplay.Details = ex.Message;
			}

			MultiView1.ActiveViewIndex = 1;
		}
	}

	private void DiscoverHttpEndpoint(Identifier identifier, out Uri providerEndpoint, out Version version) {
		providerEndpoint = null;
		version = null;

		List<ServiceEndpoint> endpoints = identifier.Discover(rp.Channel.WebRequestHandler).ToList();
		foreach (ServiceEndpoint endpoint in endpoints) {
			if (endpoint.ProviderEndpoint.Scheme == "http") {
				providerEndpoint = endpoint.ProviderEndpoint;
				version = ((IXrdsProviderEndpoint)endpoint).Version;
				return;
			}
		}
		if (endpoints.Count > 0) {
			// No HTTP endpoint.  Make one up by changing an HTTPS one to HTTP.
			UriBuilder endpoint = new UriBuilder(endpoints[0].ProviderEndpoint);
			endpoint.Scheme = "http";
			endpoint.Port = 80;
			providerEndpoint = endpoint.Uri;
			version = ((IXrdsProviderEndpoint)endpoints[0]).Version;
		}
	}
}