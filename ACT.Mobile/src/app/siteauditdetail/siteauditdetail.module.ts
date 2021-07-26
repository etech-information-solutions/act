import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

import { IonicModule } from '@ionic/angular';

import { SiteauditdetailPageRoutingModule } from './siteauditdetail-routing.module';

import { SiteauditdetailPage } from './siteauditdetail.page';

@NgModule({
  imports: [
    CommonModule,
    FormsModule,
    IonicModule,
    SiteauditdetailPageRoutingModule
  ],
  declarations: [SiteauditdetailPage]
})
export class SiteauditdetailPageModule {}
