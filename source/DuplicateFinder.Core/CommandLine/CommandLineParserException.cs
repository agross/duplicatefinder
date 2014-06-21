using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace DuplicateFinder.Core.CommandLine
{
  [Serializable]
  class CommandLineParserException : Exception
  {
    readonly string[] _messages;

    public CommandLineParserException(IEnumerable<string> messages) : base(null)
    {
      _messages = messages.ToArray();
    }

    protected CommandLineParserException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
      _messages = info.GetValue("Messages", typeof(Type)) as string[];
    }

    public string[] Messages
    {
      get
      {
        return _messages;
      }
    }

    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
      base.GetObjectData(info, context);
      info.AddValue("Messages", Messages);
    }
  }
}
