// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNetCore.Http.Features.IFeatureCollection
// Assembly: Microsoft.AspNetCore.Http.Features, Version=1.0.0.0, Culture=neutral, PublicKeyToken=adb9793829ddae60
// MVID: 97A77C4C-7F80-4DCD-AD67-75A33BD657C6
// Assembly location: C:\Users\HeabKing\.nuget\packages\Microsoft.AspNetCore.Http.Features\1.0.0\lib\netstandard1.3\Microsoft.AspNetCore.Http.Features.dll

using System;
using System.Collections;
using System.Collections.Generic;

// ReSharper disable once CheckNamespace
namespace Sinx.AspNetCore.Http.Features
{
	/// <summary>Represents a collection of HTTP features.</summary>
	/// <remarks>
	/// 特点: 各种类型的特性的集合
	/// </remarks>
	public interface IFeatureCollection : IEnumerable<KeyValuePair<Type, object>>, IEnumerable
	{
		/// <summary>Indicates if the collection can be modified.</summary>
		//bool IsReadOnly { get; }

		/// <summary>
		/// Incremented for each modification and can be used to verify cached results.
		/// </summary>
		//int Revision { get; }

		/// <summary>
		/// Gets or sets a given feature. Setting a null value removes the feature.
		/// </summary>
		/// <param name="key"></param>
		/// <returns>The requested feature, or null if it is not present(存在).</returns>
		object this[Type key] { get; set; }

		/// <summary>Retrieves the requested feature from the collection.</summary>
		/// <typeparam name="TFeature">The feature key.</typeparam>
		/// <returns>The requested feature, or null if it is not present.</returns>
		TFeature Get<TFeature>();

		/// <summary>Sets the given feature in the collection.</summary>
		/// <typeparam name="TFeature">The feature key.</typeparam>
		/// <param name="instance">The feature value.</param>
		void Set<TFeature>(TFeature instance);
	}
}
