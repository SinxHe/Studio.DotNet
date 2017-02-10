// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Sinx.AspNetCore.Http.Internal;
using Sinx.AspNetCore.WebUtilities;

namespace Sinx.AspNetCore.Http.Features
{
    public class QueryFeature : IQueryFeature
    {
        private FeatureReferences<IHttpRequestFeature> _features;

        private string _original;
        private IQueryCollection _parsedValues;

        public QueryFeature(IQueryCollection query)
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }

            _parsedValues = query;
        }

        public QueryFeature(IFeatureCollection features)
        {
            if (features == null)
            {
                throw new ArgumentNullException(nameof(features));
            }

            _features = new FeatureReferences<IHttpRequestFeature>(features);
        }

        private IHttpRequestFeature HttpRequestFeature =>
            _features.Fetch(ref _features.Cache, f => null);

        public IQueryCollection Query
        {
            get
            {
                if (_features.Collection == null)
                {
	                return _parsedValues ?? (_parsedValues = QueryCollection.Empty);
                }

                var current = HttpRequestFeature.QueryString;
	            if (_parsedValues != null && string.Equals(_original, current, StringComparison.Ordinal))
		            return _parsedValues;

	            _original = current;
	            var result = QueryHelpers.ParseNullableQuery(current);
	            _parsedValues = result == null ? QueryCollection.Empty : new QueryCollection(result);
	            return _parsedValues;
            }
            set
            {
                _parsedValues = value;
                if (_features.Collection != null)
                {
                    if (value == null)
                    {
                        _original = string.Empty;
                        HttpRequestFeature.QueryString = string.Empty;
                    }
                    else
                    {
                        _original = QueryString.Create(_parsedValues).ToString();
                        HttpRequestFeature.QueryString = _original;
                    }
                }
            }
        }
    }
}