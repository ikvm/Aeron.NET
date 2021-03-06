﻿/*
 * Copyright 2014 - 2017 Adaptive Financial Consulting Ltd
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0S
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using Adaptive.Agrona;

namespace Adaptive.Aeron.Command
{
    /// <summary>
    /// Control message flyweight for any message that needs to represent a connection
    /// 
    /// <para>
    /// 0                   1                   2                   3
    /// 0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1
    /// +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
    /// |                        Correlation ID                         |
    /// |                                                               |
    /// +---------------------------------------------------------------+
    /// |                          Stream ID                            |
    /// +---------------------------------------------------------------+
    /// |                        Channel Length                         |
    /// +---------------------------------------------------------------+
    /// |                           Channel (ASCII)                    ...
    /// ...                                                             |
    /// +---------------------------------------------------------------+
    /// </para>
    /// </summary>
    public class ImageMessageFlyweight
    {
        private const int CORRELATION_ID_OFFSET = 0;
        private const int STREAM_ID_FIELD_OFFSET = 8;
        private const int CHANNEL_OFFSET = 12;

        private IMutableDirectBuffer buffer;
        private int offset;
        private int lengthOfChannel;

        /// <summary>
        /// Wrap the buffer at a given offset for updates.
        /// </summary>
        /// <param name="buffer"> to wrap </param>
        /// <param name="offset"> at which the message begins. </param>
        /// <returns> for fluent API </returns>
        public ImageMessageFlyweight Wrap(IMutableDirectBuffer buffer, int offset)
        {
            this.buffer = buffer;
            this.offset = offset;

            return this;
        }

        /// <summary>
        /// return correlation id field </summary>
        /// <returns> correlation id field </returns>
        public long CorrelationId()
        {
            return buffer.GetLong(offset + CORRELATION_ID_OFFSET);
        }

        /// <summary>
        /// set correlation id field </summary>
        /// <param name="correlationId"> field value </param>
        /// <returns> flyweight </returns>
        public ImageMessageFlyweight CorrelationId(long correlationId)
        {
            buffer.PutLong(offset + CORRELATION_ID_OFFSET, correlationId);

            return this;
        }

        /// <summary>
        /// return stream id field
        /// </summary>
        /// <returns> stream id field </returns>
        public int StreamId()
        {
            return buffer.GetInt(offset + STREAM_ID_FIELD_OFFSET);
        }

        /// <summary>
        /// set stream id field
        /// </summary>
        /// <param name="streamId"> field value </param>
        /// <returns> flyweight </returns>
        public ImageMessageFlyweight StreamId(int streamId)
        {
            buffer.PutInt(offset + STREAM_ID_FIELD_OFFSET, streamId);

            return this;
        }

        /// <summary>
        /// Get the channel field as ASCII
        /// </summary>
        /// <returns> channel field </returns>
        public string Channel()
        {
            int length = buffer.GetInt(offset + CHANNEL_OFFSET);
            lengthOfChannel = BitUtil.SIZE_OF_INT + length;

            return buffer.GetStringAscii(offset + CHANNEL_OFFSET, length);
        }

        /// <summary>
        /// Set the channel field as ASCII
        /// </summary>
        /// <param name="channel"> field value </param>
        /// <returns> flyweight </returns>
        public ImageMessageFlyweight Channel(string channel)
        {
            lengthOfChannel = buffer.PutStringAscii(offset + CHANNEL_OFFSET, channel);

            return this;
        }

        /// <summary>
        /// Get the length of the current message
        /// 
        /// NB: must be called after the data is written in order to be accurate.
        /// </summary>
        /// <returns> the length of the current message </returns>
        public int Length()
        {
            return CHANNEL_OFFSET + lengthOfChannel;
        }
    }
}