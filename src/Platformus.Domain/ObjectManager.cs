﻿// Copyright © 2015 Dmitry Sikorsky. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using Platformus.Barebone;
using Platformus.Domain.Data.Abstractions;
using Platformus.Domain.Data.Models;
using Platformus.Globalization;
using Platformus.Globalization.Data.Abstractions;
using Platformus.Globalization.Data.Models;

namespace Platformus.Domain
{
  public class ObjectManager
  {
    private IRequestHandler requestHandler;

    public ObjectManager(IRequestHandler requestHandler)
    {
      this.requestHandler = requestHandler;
    }

    public IEnumerable<string> GetDisplayProperties(Object @object)
    {
      List<string> properties = new List<string>();
      Culture neutralCulture = CultureManager.GetNeutralCulture(this.requestHandler.Storage);
      Culture defaultCulture = CultureManager.GetDefaultCulture(this.requestHandler.Storage);

      if (defaultCulture != null)
      {
        foreach (Member member in this.requestHandler.Storage.GetRepository<IMemberRepository>().FilteredByClassIdInlcudingParentPropertyVisibleInList(@object.ClassId))
        {
          Property property = this.requestHandler.Storage.GetRepository<IPropertyRepository>().WithObjectIdAndMemberId(@object.Id, member.Id);

          if (property == null)
            properties.Add(string.Empty);

          else
          {
            Localization localization = null;

            if (member.IsPropertyLocalizable == true && defaultCulture != null)
              localization = this.requestHandler.Storage.GetRepository<ILocalizationRepository>().WithDictionaryIdAndCultureId((int)property.StringValueId, defaultCulture.Id);

            else if (neutralCulture != null)
              localization = this.requestHandler.Storage.GetRepository<ILocalizationRepository>().WithDictionaryIdAndCultureId((int)property.StringValueId, neutralCulture.Id);

            if (localization == null)
              properties.Add(string.Empty);

            else properties.Add(localization.Value);
          }
        }
      }

      return properties;
    }
  }
}