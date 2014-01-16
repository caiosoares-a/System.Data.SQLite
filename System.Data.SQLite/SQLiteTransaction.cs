﻿/********************************************************
 * ADO.NET 2.0 Data Provider for SQLite Version 3.X
 * Written by Robert Simpson (robert@blackcastlesoft.com)
 * 
 * Released to the public domain, use at your own risk!
 ********************************************************/

namespace System.Data.SQLite
{
  using System;
  using System.Data;
  using System.Data.Common;

  /// <summary>
  /// SQLite implementation of DbTransaction.
  /// </summary>
  public sealed class SQLiteTransaction : DbTransaction
  {
    private SQLiteConnection _cnn;

    internal SQLiteTransaction(SQLiteConnection cnn)
    {
      try
      {
        cnn._sql.Execute("BEGIN");
        _cnn = cnn;
      }
      catch (SQLiteException e)
      {
        BaseDispose();
        throw (e);
      }
    }

    /// <summary>
    /// Commits the current transaction.
    /// </summary>
    public override void Commit()
    {
      if (_cnn == null)
        throw new ArgumentNullException();

      try
      {
        _cnn._sql.Execute("COMMIT");
      }
      catch (SQLiteException e)
      {
        BaseDispose();
        throw (e);
      }
      BaseDispose();
    }

    /// <summary>
    /// Returns the underlying connection to which this transaction applies.
    /// </summary>
    protected override DbConnection DbConnection
    {
      get { return _cnn; }
    }

    /// <summary>
    /// Disposes the transaction.  If it is currently active, any changes are rolled back.
    /// </summary>
    public override void Dispose()
    {
      if (_cnn != null) 
        Rollback();
      GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Gets the isolation level of the transaction.  SQLite does not support isolation levels, so this always returns Unspecified.
    /// </summary>
    public override IsolationLevel IsolationLevel
    {
      get { return IsolationLevel.Unspecified; }
    }

    /// <summary>
    /// Rolls back the active transaction.
    /// </summary>
    public override void Rollback()
    {
      if (_cnn == null)
        throw new ArgumentNullException();

      try
      {
        _cnn._sql.Execute("ROLLBACK");
      }
      catch (SQLiteException e)
      {
        BaseDispose();
        throw (e);
      }
      BaseDispose();
    }

    private void BaseDispose()
    {
      _cnn._activeTransaction = null;
      _cnn = null;
    }
  }
}
