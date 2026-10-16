export interface PipingLineSummary {
  id: number;
  lineNumber: string;
  location: string | null;
  fromDescription: string | null;
  toDescription: string | null;
  pipeSizeInch: number | null;
  service: string | null;
  material: string | null;
}

export interface PipingLineWrite {
  lineNumber: string;
  location: string | null;
  fromDescription: string | null;
  toDescription: string | null;
  drawingNumber: string | null;
  service: string | null;
  material: string | null;
  inserviceDate: string | null;
  pipeSizeInch: number | null;
  originalThicknessMm: number | null;
  stress: number | null;
  jointEfficiency: number | null;
  corrosionAllowanceMm: number | null;
  designLife: number | null;
  designPressure: number | null;
  operatingPressure: number | null;
  designTemperature: number | null;
  operatingTemperature: number | null;
}

export interface ThicknessReading {
  id: number;
  inspectionDate: string;
  actualThicknessMm: number;
}

export interface TestPoint {
  id: number;
  tpNumber: number;
  tpDescription: number;
  note: string | null;
  readings: ThicknessReading[];
}

export interface Cml {
  id: number;
  cmlNumber: number;
  description: string;
  actualOutsideDiameterMm: number | null;
  designThicknessMm: number | null;
  designThicknessStatus: string;
  structuralThicknessMm: number | null;
  requiredThicknessMm: number | null;
  requiredThicknessStatus: string;
  testPoints: TestPoint[];
}

export interface PipingLineDetail extends PipingLineWrite {
  id: number;
  cmls: Cml[];
}

export interface CmlWrite {
  cmlNumber: number;
  description: string;
}
export interface TestPointWrite {
  tpNumber: number;
  tpDescription: number;
  note: string | null;
}
export interface ThicknessReadingWrite {
  inspectionDate: string;
  actualThicknessMm: number;
}
