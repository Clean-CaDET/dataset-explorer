﻿using DataSetExplorer.Core.Annotations.Model;

namespace DataSetExplorer.Core.Auth.Repository
{
    public interface IAuthRepository
    {
        Annotator Save(Annotator annotator);
        Annotator GetAnnotatorByEmail(string email);
        Annotator GetAnnotatorById(int id);
    }
}
