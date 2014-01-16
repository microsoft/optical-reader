/*
 * Copyright (c) 2014 Nokia Corporation. All rights reserved.
 *
 * Nokia and Nokia Connecting People are registered trademarks of Nokia Corporation.
 * Other product and company names mentioned herein may be trademarks
 * or trade names of their respective owners.
 *
 * See the license text file for license information.
 */

namespace OpticalReaderLib
{
    /// <summary>
    /// Camera parameter suggestion.
    /// </summary>
    public class ParameterSuggestion
    {
        /// <summary>
        /// True if parameters are accurate, meaning that there was data
        /// to back up the decision on the parameters, otherwise false.
        /// </summary>
        public bool IsAccurate { get; set; }

        /// <summary>
        /// Suggested viewfinder zoom factor. Zoom factor of one means that
        /// the viewfinder should not be zoomed at all and it should show
        /// the camera preview in whole as it is got from the camera.
        /// </summary>
        public double Zoom { get; set; }

        /// <summary>
        /// Optimal reading distance, how close the camera should be to the
        /// real-life target object. This may be used to guide the application
        /// user to position the device on an optical reading distance.
        /// </summary>
        public double Distance { get; set; }
    }
}
