import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

import { IonicModule } from '@ionic/angular';

import { SiteauditPageRoutingModule } from './siteaudit-routing.module';

import { SiteauditPage } from './siteaudit.page';

@NgModule({
  imports: [
    CommonModule,
    FormsModule,
    IonicModule,
    SiteauditPageRoutingModule
  ],
  declarations: [SiteauditPage]
})
export class SiteauditPageModule {}
