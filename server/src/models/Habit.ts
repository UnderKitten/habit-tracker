import mongoose from "mongoose";

export interface IHabit extends mongoose.Document {
  name: string;
  userId: mongoose.Types.ObjectId;
  description?: string;
  dateTime?: Date;
}

export interface IHabitEntry extends mongoose.Document {
  habitId: string;
  date: Date;
  isDone: Boolean;
}

const habitSchema = new mongoose.Schema<IHabit>({
  name: { type: String, required: true },
  userId: { type: mongoose.Schema.Types.ObjectId, required: true, ref: "User" },
  description: String,
  dateTime: Date,
});

export const Habit: mongoose.Model<IHabit> = mongoose.model<IHabit>(
  "Habit",
  habitSchema
);

const habitEntrySchema = new mongoose.Schema<IHabitEntry>({
  habitId: { type: String, required: true },
  date: { type: Date, required: true },
  isDone: { type: Boolean, required: true },
});

export const HabitEntry: mongoose.Model<IHabitEntry> =
  mongoose.model<IHabitEntry>("HabitEntry", habitEntrySchema);
