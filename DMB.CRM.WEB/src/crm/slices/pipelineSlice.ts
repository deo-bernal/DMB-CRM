import { createAsyncThunk, createSlice } from "@reduxjs/toolkit";
import http from "../services/http.service";
import type { Opportunity, Pipeline } from "../models";

export const fetchPipelines = createAsyncThunk("pipelines/list", async () => {
  const res = await http.get<Pipeline[]>("/pipeline/list");
  return res.data;
});

export const fetchOpportunities = createAsyncThunk("opportunities/list", async () => {
  const res = await http.get<Opportunity[]>("/opportunity/list");
  return res.data;
});

export const saveOpportunity = createAsyncThunk(
  "opportunities/save",
  async (payload: { id?: string; body: Partial<Opportunity> }) => {
    const res = payload.id
      ? await http.put<Opportunity>(`/opportunity/${payload.id}`, payload.body)
      : await http.post<Opportunity>("/opportunity", payload.body);
    return res.data;
  }
);

export const moveOpportunity = createAsyncThunk(
  "opportunities/move",
  async (payload: { id: string; stageId: string }) => {
    const res = await http.post<Opportunity>(`/opportunity/${payload.id}/move`, {
      stageId: payload.stageId,
    });
    return res.data;
  }
);

const slice = createSlice({
  name: "pipelines",
  initialState: { pipelines: [] as Pipeline[], opportunities: [] as Opportunity[] },
  reducers: {},
  extraReducers: (builder) => {
    builder
      .addCase(fetchPipelines.fulfilled, (state, action) => {
        state.pipelines = action.payload;
      })
      .addCase(fetchOpportunities.fulfilled, (state, action) => {
        state.opportunities = action.payload;
      })
      .addCase(saveOpportunity.fulfilled, (state, action) => {
        const idx = state.opportunities.findIndex((o) => o.id === action.payload.id);
        if (idx >= 0) state.opportunities[idx] = action.payload;
        else state.opportunities.unshift(action.payload);
      })
      .addCase(moveOpportunity.fulfilled, (state, action) => {
        const idx = state.opportunities.findIndex((o) => o.id === action.payload.id);
        if (idx >= 0) state.opportunities[idx] = action.payload;
      });
  },
});

export default slice.reducer;
