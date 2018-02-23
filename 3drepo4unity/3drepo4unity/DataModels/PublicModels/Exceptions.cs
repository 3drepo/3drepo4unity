/*
 *	Copyright (C) 2018 3D Repo Ltd
 *
 *	This program is free software: you can redistribute it and/or modify
 *	it under the terms of the GNU Affero General Public License as
 *	published by the Free Software Foundation, either version 3 of the
 *	License, or (at your option) any later version.
 *
 *	This program is distributed in the hope that it will be useful,
 *	but WITHOUT ANY WARRANTY; without even the implied warranty of
 *	MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *	GNU Affero General Public License for more details.
 *
 *	You should have received a copy of the GNU Affero General Public License
 *	along with this program.  If not, see <http://www.gnu.org/licenses/>.
 * 
 *  Author: Sebastian J Friston
 */

using System;

namespace RepoForUnity
{
    public class RepoModelLoadingException : Exception
    {
        public RepoModelLoadingException(string err) : base(err) { }
    }

    public class RepoNoValueException : Exception
    {
        public RepoNoValueException(string err) : base(err) { }
    }

    public class RepoUnsupportedException : Exception
    {
        public RepoUnsupportedException(string err  = "This version of 3drepo4unity is no longer supported. Please update.") : base(err) { }
    }
}
