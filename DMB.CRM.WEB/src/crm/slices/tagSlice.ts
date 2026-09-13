import { createAsyncThunk, createSlice } from "@reduxjs/toolkit";
import http from "../services/http.service";
import type { Tag } from "../models";

export const fetchTags = createAsyncThunk("tags/list", async () => {
  const res = await http.get<Tag[]>("/tag/list");
  return res.data;
});

export const saveTag = createAsyncThunk("tags/save", async (body: Partial<Tag> & { id?: string }) => {
  const res = body.id
    ? await http.put<Tag>(`/tag/${body.id}`, body)
    : await http.post<Tag>("/tag", body);
  return res.data;
});

const slice = createSlice({
  name: "tags",
  initialState: { items: [] as Tag[] },
  reducers: {},
  extraReducers: (builder) => {
    builder
      .addCase(fetchTags.fulfilled, (state, action) => {
        state.items = action.payload;
      })
      .addCase(saveTag.fulfilled, (state, action) => {
        const idx = state.items.findIndex((t) => t.id === action.payload.id);
        if (idx >= 0) state.items[idx] = action.payload;
        else state.items.push(action.payload);
      });
  },
});

export default slice.reducer;
