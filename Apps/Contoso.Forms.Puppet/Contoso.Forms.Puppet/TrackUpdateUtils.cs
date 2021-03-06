// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using Microsoft.AppCenter.Distribute;
using Xamarin.Forms;

namespace Contoso.Forms.Puppet
{
    public class TrackUpdateUtils
    {
        public const UpdateTrack DefaultUpdateTrackType = UpdateTrack.Public;

        public static UpdateTrack? GetPersistedUpdateTrack()
        {
            if (Application.Current.Properties.TryGetValue(Constants.UpdateTrackKey, out object persistedObject))
            {
                string persistedString = (string)persistedObject;
                if (Enum.TryParse<UpdateTrack>(persistedString, out var persistedEnum))
                {
                    return persistedEnum;
                }
            }
            return null;
        }

        public static async System.Threading.Tasks.Task SetPersistedUpdateTrackAsync(UpdateTrack choice)
        {
            Application.Current.Properties[Constants.UpdateTrackKey] = choice.ToString();
            await Application.Current.SavePropertiesAsync();
        }

        public static IEnumerable<string> GetUpdateTrackChoiceStrings()
        {
            foreach (var updateTrackObject in Enum.GetValues(typeof(UpdateTrack)))
            {
                yield return updateTrackObject.ToString();
            }
        }

        public static int ToPickerUpdateTrackIndex(UpdateTrack updateTrack)
        {
            return (int)(updateTrack - 1);
        }

        public static UpdateTrack FromPickerUpdateTrackIndex(int index)
        {
            return (UpdateTrack)(index + 1);
        }
    }
}
