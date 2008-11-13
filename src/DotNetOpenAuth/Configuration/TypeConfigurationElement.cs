﻿//-----------------------------------------------------------------------
// <copyright file="TypeConfigurationElement.cs" company="Andrew Arnott">
//     Copyright (c) Andrew Arnott. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace DotNetOpenAuth.Configuration {
	using System;
	using System.Configuration;

	/// <summary>
	/// Represents an element in a .config file that allows the user to provide a @type attribute specifying
	/// the full type that provides some service used by this library.
	/// </summary>
	/// <typeparam name="T">A constraint on the type the user may provide.</typeparam>
	internal class TypeConfigurationElement<T> : ConfigurationElement {
		/// <summary>
		/// The name of the attribute whose value is the full name of the type the user is specifying.
		/// </summary>
		private const string CustomTypeConfigName = "type";

		/// <summary>
		/// Initializes a new instance of the TypeConfigurationElement class.
		/// </summary>
		public TypeConfigurationElement() {
		}

		/// <summary>
		/// Gets or sets the full name of the type.
		/// </summary>
		/// <value>The full name of the type, such as: "ConsumerPortal.Code.CustomStore, ConsumerPortal".</value>
		[ConfigurationProperty(CustomTypeConfigName)]
		////[SubclassTypeValidator(typeof(T))] // this attribute is broken in .NET, I think.
		public string TypeName {
			get { return (string)this[CustomTypeConfigName]; }
			set { this[CustomTypeConfigName] = value; }
		}

		/// <summary>
		/// Gets the type described in the .config file.
		/// </summary>
		public Type CustomType {
			get { return string.IsNullOrEmpty(this.TypeName) ? null : Type.GetType(this.TypeName); }
		}

		/// <summary>
		/// Creates an instance of the type described in the .config file.
		/// </summary>
		/// <param name="defaultValue">The value to return if no type is given in the .config file.</param>
		/// <returns>The newly instantiated type.</returns>
		public T CreateInstance(T defaultValue) {
			return this.CustomType != null ? (T)Activator.CreateInstance(this.CustomType) : defaultValue;
		}
	}
}
