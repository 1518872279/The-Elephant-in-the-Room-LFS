# Contact Preview System

This system allows you to display message previews from specific contacts based on the current game day and day part.

## Overview

The ContactPreview script shows messages from a specific contact in a UI text component. The message content depends on:
- Current day in the game
- Current day part (Morning/Evening)
- Contact name

## Setup

### 1. TimedMessage Structure

Each message now includes a `day` field to specify which day it should appear:

```csharp
[System.Serializable]
public class TimedMessage
{
    public DayPartManager.DayPart triggerPart;  // Morning or Evening
    public int day;  // Which day this message should appear (1, 2, 3, etc.)
    public string senderName;
    [TextArea] public string messageText;
    public string replyOption1;
    public string replyOption2;
}
```

### 2. ContactPreview Component

Add the ContactPreview script to a GameObject with the following UI components:

- **ContactName** (TextMeshProUGUI): Displays the contact's name
- **MessagePreview** (TextMeshProUGUI): Shows the message preview (first 100 characters)
- **ButtonToReplyInterface** (Button): Button to open the full message interface

### 3. Configuration

In the ContactPreview component inspector:

1. **Contact Name**: Set the name of the contact to show messages for (e.g., "Contact A")
2. **UI References**: Assign the TextMeshProUGUI and Button components

## How It Works

1. **Automatic Updates**: The preview updates automatically when the day part changes
2. **Message Filtering**: Only shows messages that match:
   - The specified contact name
   - The current day number
   - The current day part (Morning/Evening)
3. **Preview Display**: Shows the first 100 characters of the message with "..." if longer
4. **Empty State**: Shows blank text if no message exists for the current conditions

## Example Usage

### Setting up a Contact Preview

1. Create a UI GameObject with TextMeshProUGUI components
2. Add the ContactPreview script
3. Configure the contact name (e.g., "Alice")
4. Assign the UI references

### Adding Messages

In MessageManager, add messages like this:

```csharp
TimedMessage message = new TimedMessage();
message.day = 5;  // Day 5
message.triggerPart = DayPartManager.DayPart.Morning;  // Morning
message.senderName = "Alice";  // Contact name
message.messageText = "Good morning! How are you doing today?";
message.replyOption1 = "I'm doing great!";
message.replyOption2 = "Could be better...";
```

### Multiple Contacts

Create multiple ContactPreview components for different contacts:

- ContactPreview for "Alice" (shows Alice's messages)
- ContactPreview for "Bob" (shows Bob's messages)
- etc.

## API Methods

### ContactPreview

- `UpdateMessagePreview()`: Manually refresh the preview
- `SetContactName(string name)`: Change the contact name
- `ManualUpdatePreview()`: Context menu option for testing

### MessageManager

- `GetMessagesForContact(string contactName, int day, DayPart dayPart)`: Get messages for specific contact/day/part
- `ShowMessageInMainPanel(TimedMessage msg)`: Display a message in the main interface

## Testing

Use the context menu "Update Message Preview" to manually test the preview functionality.

The system will log debug information to help track message filtering and display. 